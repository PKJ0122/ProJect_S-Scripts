using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 모든 UI 관리, 전체 스크린용 및 팝업용 UI 추가로 관리
/// </summary>
public class UIManager : SingletonMonoBase<UIManager>
{
    Dictionary<Type, IUI> _uis = new Dictionary<Type, IUI>();  // 타입으로 UI 검색
    List<IUI> _screens = new List<IUI>();  // 활성화 되어있는 Screen UI 들
    Stack<IUI> _popups = new Stack<IUI>(); // 활성화 되어있는 Popup UI 들


    private void Update()
    {
        UpdateInputActions();
        if (_popups == null)
            Debug.LogWarning("없음");
    }

    /// <summary>
    /// 현재 상호작용 가능한 UI 의 상호작용 업데이트
    /// </summary>
    public void UpdateInputActions()
    {
        // 활성화된 Popup이 존재한다면 최상단 Popup UI 만 상호작용
        if (_popups.Count > 0)
        {
            if (_popups.Peek().inputActionEnable)
            {
                _popups.Peek().InputAction();
            }
        }

        // 활성화된 Screen UI 가 존재한다면 모두 상호작용
        for (int i = _screens.Count - 1; i >= 0; i--)
        {
            if (_screens[i].inputActionEnable)
            {
                _screens[i].InputAction();
            }
        }
    }

    /// <summary>
    /// 스크린용 UI 등록
    /// </summary>
    public void RegisterScreen(IUI ui)
    {
        if (_uis.TryAdd(ui.GetType(), ui)) { }
        else
        {
            throw new Exception($"[UIManager] : UI 등록 실패. {ui.GetType()} 는 이미 등록되어있습니다.");
        }
    }

    /// <summary>        
    /// 팝업용 UI 등록        
    /// </summary>
    public void RegisterPopup(IUI ui)
    {
        if (_uis.TryAdd(ui.GetType(), ui)) { }
        else
        {
            throw new Exception($"[UIManager] : UI 등록 실패. {ui.GetType()} 는 이미 등록되어있습니다.");
        }
    }

    /// <summary>
    /// Resolve 기능. 원하는 UI 가져오는 함수
    /// </summary>
    /// <typeparam name="T"> 가져오고 싶은 UI 타입 Type 객체 </typeparam>
    public T Get<T>()
        where T : IUI
    {
        return (T)_uis[typeof(T)];
    }

    /// <summary>
    /// 새로 활성화될 Popup UI을 관리
    /// </summary>
    /// /// <typeparam name="ui"> 활성화될 UI </typeparam>
    public void PushPopup(IUI ui)
    {
        if (_popups.Count > 0)
        {
            _popups.Peek().inputActionEnable = false; // 기존에 있던 팝업 입력 불가능하게
        }

        _popups.Push(ui);
        _popups.Peek().inputActionEnable = true;  // 새로운 팝업 입력 가능하게
        _popups.Peek().sortingOrder = _popups.Count; // 새로운 팝업 최상단으로
    }

    /// <summary>
    /// 닫으려는 Popup UI 관리
    /// </summary>
    /// <exception cref="Exception">Popup UI가 최상단에 없을 경우</exception>
    public void PopPopup(IUI ui)
    {
        if (_popups.Peek() != ui)
        {
            throw new Exception($"[UIManager] : {ui.GetType()} 팝업을 닫기 시도했지만 최상단이 아님");
        }

        _popups.Pop().inputActionEnable = false;
        if (_popups.Count > 0)
        {
            _popups.Peek().inputActionEnable = true;  // 새로운 팝업 입력 가능하게
        }
    }

    public void PushScreen(IUI ui)
    {
        _screens.Add(ui);
        ui.inputActionEnable = true;
    }

    public void PopScreen(IUI ui)
    {
        _screens.Remove(ui);
        ui.inputActionEnable = false;
    }
}


using System.Collections;

public interface IItemEvent
{
    bool isActive { set; }

    IEnumerator ItemEvent();
}

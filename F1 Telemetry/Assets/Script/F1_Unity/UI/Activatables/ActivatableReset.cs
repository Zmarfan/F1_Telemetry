using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatableReset
{
    /// <summary>
    /// Resets all stored info about this activatable
    /// </summary>
    void ClearActivatable();
}

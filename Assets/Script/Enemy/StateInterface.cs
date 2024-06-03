using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    // 상태가 시작될때 호출됨
    public void EnterState();

    // 매 프레임 마다 호출됨
    public void UpdateState();

    // 상태가 종료될때 호출됨
    public void ExitState();
}
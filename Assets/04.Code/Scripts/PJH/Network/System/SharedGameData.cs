using System;
using System.Collections.Generic;
using Fusion;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class SharedGameData : NetworkBehaviour
{
   public static SharedGameData Instance;
   
   public static int ReadyCount { get; private set; }
   public static int GameEndCount { get; private set; }
   public static bool StartGame { get; private set; }
   public static int BlockCount { get; private set; }

   private void Start()
   {
      if (HasStateAuthority)
      {
         Debug.Log("이 클라이언트가 해당 오브젝트의 StateAuthority임");
      }
      else
      {
         Debug.Log("이 클라이언트는 해당 오브젝트의 StateAuthority가 아님");
      }
   }
   
   public override void Spawned()
   {
      if (!HasStateAuthority)
         return;
      Instance = this;
      
      ReadyCount = 0;
      GameEndCount = 0;
   }
    
   [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void RpcReady(RpcInfo info = default)
   {
      ReadyCount++;
      Debug.Log($"ReadyCount Changed : {ReadyCount}");
   }

   [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void RpcWait(RpcInfo info = default)
   {
      ReadyCount--;
      Debug.Log($"ReadyCount Changed : {ReadyCount}");
   }
   
   [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void GameStartRpc(RpcInfo info = default)
   {
      StartGame = true;
      RunnerManager.Instance.runner.SessionInfo.IsOpen = false;
      RunnerManager.Instance.runner.SessionInfo.IsVisible = false;
      Debug.Log($"ReadyCount Changed : {ReadyCount}");
   }
   
   [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void BlockCountRpc(RpcInfo info = default)
   {
      BlockCount++;
      Debug.Log($"BlockCount Changed : {BlockCount}");
   }

   
    
   [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void RpcGameEnd(RpcInfo info = default)
   {
      GameEndCount++;
      Debug.Log($"GameEndCount Changed : {GameEndCount}");
   }
   
}

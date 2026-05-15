using UnityEngine;
using Fusion;

public class FlamePuzzleNetworkPlayer : NetworkBehaviour
{
    [Networked]
    public int PlayerID { get; set; }

    public FlamePuzzlePlayerState playerState;

    public static FlamePuzzleNetworkPlayer Local;

    public override void Spawned()
    {

        if (Object.HasInputAuthority)
        {
            Local = this;
        }

        // Assign unique player id.
        PlayerID = Runner.LocalPlayer.PlayerId;

        // Debug.
        playerState.debugText.text += "\nNetwork Player ID: " + PlayerID;
    }
}
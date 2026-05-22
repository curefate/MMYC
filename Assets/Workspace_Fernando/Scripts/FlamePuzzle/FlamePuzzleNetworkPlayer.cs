using Fusion;
using TMPro;
using UnityEngine;

public class FlamePuzzleNetworkPlayer : NetworkBehaviour
{
    public static FlamePuzzleNetworkPlayer Local;

    public FlamePuzzlePlayerState playerState;

    public TMP_Text debugText;

    public override void Spawned()
    {
        //if (Object.HasStateAuthority)
        if (Object.HasInputAuthority)
        {
            Local = this;
        }

        debugText.text +=
            "\nNETWORK PLAYER SPAWNED";

        if (Object.HasInputAuthority)
        {
            Local = this;

            debugText.text +=
                "\nLOCAL PLAYER SET";
        }
    }
}
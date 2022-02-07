using BigHead.Framework.Game;

public partial class Res : MonoGlobalSingleton<Res>
{
    private GameListener _listener;
    
    private void Awake()
    {
        _listener = BH.SystemListener;
        Awake_Hotfix();
    }

    private void OnDestroy()
    {
        OnDestroy_Hotfix();
    }

    partial void Awake_Hotfix();
    partial void OnDestroy_Hotfix();
}
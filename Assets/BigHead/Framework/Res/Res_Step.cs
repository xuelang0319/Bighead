using BigHead.Framework.Game;

public partial class Res : MonoGlobalSingleton<Res>
{
    private Listener _listener;
    
    private void Awake()
    {
        _listener = Listener.Instance;
        Awake_Hotfix();
    }

    private void OnDestroy()
    {
        OnDestroy_Hotfix();
    }

    partial void Awake_Hotfix();
    partial void OnDestroy_Hotfix();
}
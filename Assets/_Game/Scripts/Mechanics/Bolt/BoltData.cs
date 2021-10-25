namespace Mechanics.Bolt
{
    /// Summary:
    /// The Bolt Data struct that is used by the Bolt Controller
    /// This scripts main purpose is to hold data such as the Bolt Controller and the Player Controller
    /// It is passed through the OnWarpBoltImpact function
    public struct BoltData
    {
        public bool Valid;
        public BoltManager BoltManager;
        public PlayerController PlayerController;

        public BoltData(BoltManager manager, PlayerController player)
        {
            BoltManager = manager;
            PlayerController = player;
            Valid = manager != null && player != null;
        }
    }
}
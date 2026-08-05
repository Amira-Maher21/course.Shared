using System.ComponentModel.DataAnnotations;

namespace  Shared.Domain.Models.AppSystem
{

    public class AppServiceVersion
    {
        [Key]
        public int ServiceVersionId { get; set; }

        public int ServiceId { get; set; }
        public string? Version { get; set; }
        public int? VersionState { get; set; }
    }

    public class VersionState
    {
        private string _state;
        private int _stateNo;
        private static readonly Dictionary<int, string> _states =
            new Dictionary<int, string> {
                {1,"Current"},
                {2,"Beta" },
                {3,"Deprecated" },
                {4,"Archived" },
                {5,"Development" },
            };
        private VersionState(int stateNo)
        {
            if (_states.Any(s => s.Key == stateNo))
            {
                _state = _states[stateNo];
                _stateNo = stateNo;
            }
            else
            {
                throw new Exception($"Invalid state number [{stateNo}]");

            }

        }

        public static VersionState Current { get { return new VersionState(1); } }
        public static VersionState Beta { get { return new VersionState(2); } }
        public static VersionState Deprecated { get { return new VersionState(3); } }
        public static VersionState Archived { get { return new VersionState(4); } }
        public static VersionState Development { get { return new VersionState(5); } }

        public static Dictionary<int, string> AllStates()
        {
            return _states;
        }


        public static implicit operator int(VersionState state) { return state._stateNo; }
        public static implicit operator VersionState(int stateNo) { return new VersionState(stateNo); }

        public override string ToString()
        {
            return _state;
        }
    }
}

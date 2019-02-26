using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class ActionValidator4 : IActionValidator
    {
        public int Property1 { get; set; }
        public bool GetIsEnabled(int actionId)
        {
            switch(actionId)
            {
                case 1:
                case 3:
                    return true;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj is ActionValidator4 actionValidator4)
                return this.Property1 == actionValidator4.Property1;

            return false;
        }
    }
}
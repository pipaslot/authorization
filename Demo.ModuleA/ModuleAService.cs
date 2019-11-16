using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Authorization;

namespace Demo.ModuleA
{
    public class ModuleAService
    {
        private readonly IUser<long> _user;

        public ModuleAService(IUser<long> user)
        {
            _user = user;
        }

        public void CreateModuleA()
        {
             _user.CheckPermission(ModuleAPermission.Create);
            //do secured operation
        }

        public  void UpdateModuleA()
        {
            if (_user.IsAllowed(ModuleAPermission.Update))
            {
                //do secured operation
                return;
            }

            throw new Exception("Sorry dude, you can't");
        }

        public void DoBusinessLogic()
        {
            if (_user.IsAllowed(ModuleAPermission.DoItAsGood))
            {
                //override standard business logic rule
            }
        }
    }
}

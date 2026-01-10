
using Data_Layer.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class UserBusiness 
    {
        public UserBusiness(UserData userData)
        {
            UserData = userData;
        }

        public UserData UserData { get; }

        public async Task<bool> Add(User user)
        {
            return await UserData.Add(user);
        }
    }
}

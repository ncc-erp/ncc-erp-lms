using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using RMALMS.IoC;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RMALMS.Reports
{
    public class TrackingLogAppService : ApplicationService
    {
        private readonly IWorkScope _ws;

        public TrackingLogAppService(IWorkScope workScope) : base()
        {
            this._ws = workScope;
        }

        /// <summary>
        /// Log of User logout
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateUserLogoutInfo()
        {
            if (AbpSession.UserId == null)
                return false;
            var item = new UserLoginAttempt
            {
                Result = AbpLoginResultType.LockedOut,
                UserId = AbpSession.UserId,
                TenantId = AbpSession.TenantId,
                ClientIpAddress = GetIPAddress(),
            };
            await _ws.GetRepo<UserLoginAttempt, long>().InsertAsync(item);
            return true;
        }


        /// <summary>
        /// Get IP of client
        /// </summary>
        /// <returns></returns>
        private string GetIPAddress()
        {
            string IPAddress = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace wfwinhello
{
    class Util
    {
        const int NTE_NO_KEY = unchecked((int)0x8009000D);
        const int NTE_PERM = unchecked((int)0x80090010);

        public static async Task<bool> TryDeleteCredentialAccountAsync(string userId)
        {
            bool deleted = false;

            try
            {
                await KeyCredentialManager.DeleteAsync(userId);
                deleted = true;
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case NTE_NO_KEY:
                        // Key is already deleted. Ignore this error.
                        break;
                    case NTE_PERM:
                        // Access is denied. Ignore this error. We tried our best.
                        break;
                    default:
                        throw;
                }
            }
            return deleted;
        }
    }
}

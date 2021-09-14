using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IRefreshtokensBL
    {
        Refreshtokens InsertRefreshtokens(Refreshtokens Refreshtokens);
        Refreshtokens UpdateRefreshtokens(Refreshtokens Refreshtokens);
        IEnumerable<Refreshtokens> GetRefreshtokens();
        bool DeleteRefreshtokens(int RefreshtokensId);
        Refreshtokens GetRefreshtokens(string refreshTokenFromUser);
        IQueryable<Refreshtokens> QueryRefreshtokens();
        void AddOrUpdateRefreshToken(Refreshtokens refreshToken);
        bool DeleteUserRefreshTokensByUserId(int userId);
    }
}

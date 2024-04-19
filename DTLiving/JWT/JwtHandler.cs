using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DTLiving.JWT
{

    /* 
         JWT產生 ( GenerateJwtToken ) :
            1. 接收使用者的 ID / 名稱 / 角色作為輸入。
            2. 建立了 JWT 的聲明 ( Claims ),包含使用者的 ID / 名稱 / 角色。
            3. 建立一個對稱安全金鑰(SymmetricSecurityKey),利用金鑰簽署 JWT Token
            4. 利用金鑰和 HMAC-SHA256 簽署 JWT。
            5. 建立 JWT Token,設定發行者(issuer) / 接收者(audience) / 聲明(claims) / 
                到期時間(expires) / 簽署憑證(signingCredentials),並轉換為字串返回。

         JWT驗證 ( ValidateJwtToken ) :
            1. 用於驗證給定的 JWT Token。
            2. 如果驗證成功，則返回 JWT Token 的 Subject，這裡假設 Subject 包含使用者的識別符。
            3. 如果驗證失敗，則返回 null。    
     */

    public static class JwtHandler
    {
        // JWT 密鑰
        private static readonly string JwtKey = "UHJTFRTYUY787FVGHMJYAERvlkuytnbf";
        // JWT 發行者
        private static readonly string JwtIssuer = "HelpHubAdminPanel";
        // JWT 接收者
        private static readonly string JwtAudience = "SecureApplicationUser";

        /// <summary>
        /// JWT Token 生成
        /// </summary>
        /// <param name="userId"> 使用者 Id </param>
        /// <param name="userName"> 使用者名稱 </param>
        /// <param name="role"> 使用者角色 </param>
        /// <returns> JWT Token 字串 </returns>
        public static string GenerateJwtToken(string userId, string userName, string role)
        {
            // 定義 JWT 的聲明（Claims）
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim("UserName", userName),
                new Claim(ClaimTypes.Role, role)
            };

            // 密鑰生成對稱安全金鑰
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 建立 JWT Token
            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            // 將 JWT Token 轉換成字串
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// JWT Token 驗證
        /// </summary>
        /// <param name="token"> JWT Token 字串 </param>
        /// <returns> 使用者 ID </returns>
        public static string ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtKey);

            try
            {
                // 驗證 JWT Token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // 假設 Subject 包含使用者識別符
                return jwtToken.Subject;
            }
            catch
            {
                return null;
            }
        }
    }
}

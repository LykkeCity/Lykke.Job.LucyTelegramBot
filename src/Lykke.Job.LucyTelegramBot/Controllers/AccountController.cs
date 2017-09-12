using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.Job.LucyTelegramBot.Models;
using Lykke.Job.LucyTelegramBot.Models.BackOffice.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Job.LucyTelegramBot.Controllers
{    
    public class AccountController : Controller
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly LucyTelegramBotSettings _botSettings;
        private readonly ITgEmployeeRepository _employeeRepository;

        public AccountController(GoogleAuthSettings googleAuthSettings, LucyTelegramBotSettings botSettings, ITgEmployeeRepository employeeRepository)
        {
            _googleAuthSettings = googleAuthSettings;
            _botSettings = botSettings;
            _employeeRepository = employeeRepository;
        }

        [HttpGet]        
        [Route("account/signin")]
        public IActionResult SignIn()
        {
            var model = new SignInModel
            {
                GoogleApiClientId = _googleAuthSettings.ApiClientId
            };

            return View(model);
        }
        
        [HttpPost]
        [Route("account/auth")]
        public async Task<IActionResult> Authenticate(string googleSignInIdToken)
        {
            try
            {
                var webSignature = await GoogleJsonWebSignatureEx.ValidateAsync(googleSignInIdToken);

                if (webSignature == null)
                {
                    return JsonFailResult("Technical problem. Sorry.", "#googleSignIn");
                }

                if (!string.Equals(webSignature.Audience, _googleAuthSettings.ApiClientId))
                {
                    return JsonFailResult("Technical problem. Sorry.", "#googleSignIn");
                }

                if (string.IsNullOrWhiteSpace(webSignature.Email))
                {
                    return JsonFailResult("Email is empty", "#googleSignIn");
                }

                if (!Regex.IsMatch(webSignature.Email, _googleAuthSettings.AvailableEmailsRegex))
                {
                    return JsonFailResult("Email should be at lykke", "#googleSignIn");
                }

                if (!webSignature.IsEmailValidated)
                {
                    return JsonFailResult("Email is not valid.", "#googleSignIn");
                }              

                var token = Guid.NewGuid().ToString();

                await _employeeRepository.AddUserAsync(token, webSignature?.Email);

                return Json(new { status = "Success", redirectUrl = $"http://telegram.me/{_botSettings.BotName}?start=" + token });
            }
            catch (InvalidJwtException)
            {
                return JsonFailResult("Technical problem. Sorry.", "#googleSignIn");
            }
        }

        public JsonResult JsonFailResult(string message, string div)
        {            
            return new JsonResult(new { status = "Fail", msg = message });

        }
    }
}
﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.Job.LucyTelegramBot.Models;
using Lykke.Job.LucyTelegramBot.Models.BackOffice.Helpers;
using Lykke.Job.LucyTelegramBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Job.LucyTelegramBot.Controllers
{    
    public class AccountController : Controller
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly AppSettings.LucyTelegramBotSettings _botSettings;
        private readonly ITgEmployeeRepository _employeeRepository;

        public AccountController(GoogleAuthSettings googleAuthSettings, AppSettings.LucyTelegramBotSettings botSettings, ITgEmployeeRepository employeeRepository)
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

                if (!string.Equals(webSignature.Audience, _googleAuthSettings.ApiClientId))
                {
                    return this.JsonFailResult("Technical problem. Sorry.", "#googleSignIn");
                }

                if (string.IsNullOrWhiteSpace(webSignature.Email))
                {
                    return this.JsonFailResult("Email is empty", "#googleSignIn");
                }

                if (!Regex.IsMatch(webSignature.Email, _googleAuthSettings.AvailableEmailsRegex))
                {
                    return this.JsonFailResult("Email should be at lykke", "#googleSignIn");
                }

                if (!webSignature.IsEmailValidated)
                {
                    return this.JsonFailResult("Email is not valid.", "#googleSignIn");
                }              

                var token = Guid.NewGuid().ToString();

                await _employeeRepository.AddUserAsync(token, webSignature.Email);

                return Json(new { status = "Success", redirectUrl = $"http://telegram.me/{_botSettings.BotName}?start=" + token });
            }
            catch (InvalidJwtException)
            {
                return this.JsonFailResult("Technical problem. Sorry.", "#googleSignIn");
            }
        }

        public JsonResult JsonFailResult(string message, string div)
        {            
            return new JsonResult(new { status = "Fail", msg = message });

        }
    }
}
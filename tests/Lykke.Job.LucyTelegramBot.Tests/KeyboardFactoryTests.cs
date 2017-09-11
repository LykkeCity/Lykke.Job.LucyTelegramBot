using System;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Services;
using Xunit;

namespace Lykke.Job.LucyTelegramBot.Tests
{
    public class KeyboardFactoryTests
    {
        [Fact]
        public void TestKeyboard()
        {
            var settings = new LucyTelegramBotSettings();
            settings.Commands = new[]
            {
                new LykkeBotCommand {Name = "start", Commands = new[] {"Info", "Info1", "Info2"}},
                new LykkeBotCommand {Name = "Info", Commands = new[] {"Com1", "Com2", "Com3", "Com4", "Com5"}},
                new LykkeBotCommand {Name = "Info1", Commands = Array.Empty<string>()},
                new LykkeBotCommand {Name = "Info2", Commands = new[] {"Zz"}},
                new LykkeBotCommand {Name = "Com1", Commands = Array.Empty<string>()},
                new LykkeBotCommand {Name = "Com2", Commands = Array.Empty<string>()},
                new LykkeBotCommand {Name = "Com3", Commands = Array.Empty<string>()},
                new LykkeBotCommand {Name = "Com4", Commands = Array.Empty<string>()},
                new LykkeBotCommand {Name = "Com5", Commands = Array.Empty<string>()}
            };

            var factory = new KeyboardsFactory(settings);

            var keyboard = factory.GetKeyboard("start");

            Assert.Equal(2, keyboard.Keyboard.Length);
            Assert.Equal(2, keyboard.Keyboard[0].Length);
            Assert.Equal(1, keyboard.Keyboard[1].Length);

            keyboard = factory.GetKeyboard("Info");

            Assert.Equal(3, keyboard.Keyboard.Length);
            Assert.Equal(2, keyboard.Keyboard[0].Length);
            Assert.Equal(2, keyboard.Keyboard[1].Length);
            Assert.Equal(2, keyboard.Keyboard[2].Length);

            keyboard = factory.GetKeyboard("Info1");

            Assert.Equal(2, keyboard.Keyboard.Length);
            Assert.Equal(2, keyboard.Keyboard[0].Length);
            Assert.Equal(1, keyboard.Keyboard[1].Length);

            keyboard = factory.GetKeyboard("Info2");

            Assert.Equal(1, keyboard.Keyboard.Length);
            Assert.Equal(2, keyboard.Keyboard[0].Length);
            Assert.Equal("⬅️ Back", keyboard.Keyboard[0][1].Text);
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

//Модифицированный скрипт Visteras-а
//http://steamcommunity.com/sharedfiles/filedetails/?id=918344254
//by pasvitas twitch.tv/pasvitas


namespace RutonyChat {
    public class Script {

        public void RunScript(string site, string username, string text, Dictionary<string, string> param) {

            string langstring = ProgramProps.lang;

            int lang = 0; 
            if (langstring != "ru-RU")
            {
                lang = 1;
            }

            int credit;
            int status = 0;
            RankControl.ChatterRank cr = RankControl.ListChatters.Find(r => r.Nickname == username.Trim().ToLower());

            if (cr == null) {
                if (lang == 0)
                {
                    RutonyBot.BotSay(site,
                        username + ", ошибка! Вашей записи нет в базе данных или она повреждена!");
                }    
                else
                {
                    RutonyBot.BotSay(site,
                        username + ", error! Your entry is not in the database or it's corrupted!");
                }
                return;

            }

            string textsfile = ProgramProps.dir_scripts + @"\texts.json";

            string[] filetexts = File.ReadAllLines(textsfile);

            CreditName names = JsonConvert.DeserializeObject<CreditName>(filetexts[0]);
			
			
			
			string[] arrString = text.Split(' ');
			
            
            if (arrString.Length != 3) {
                if (lang == 0)
                {
                    RutonyBot.BotSay(site, username + ", используйте !ставка (кол-во кредитов) (красное/черное/ноль)");
                }    
                else
                {
                    RutonyBot.BotSay(site, username + ", use !gamble (amount of credit) (red/black/zero)");
                }
               
                return;
            }
			//int credit = 0;
			credit = int.Parse(arrString[1]);

            if (credit <= 0) {
                if (lang == 0)
                {
                    RutonyBot.BotSay(site, username + ", кредитов должно быть больше 0!");
                }    
                else
                {
                    RutonyBot.BotSay(site, username + ", credits must be greater than 0!");
                }
               
                return;
            }
            if (cr.CreditsQty < credit) {
                if (lang == 0)
                {
                    RutonyBot.BotSay(site, username + ", у вас всего " +  cr.CreditsQty + " кредитов!");
                }    
                else
                {
                    RutonyBot.BotSay(site, username + ", you have only " +  cr.CreditsQty + " credits!");
                }
                return;
            }

            if (credit > int.Parse(param["Maximum"]))
            {
                if (lang == 0)
                {
                    RutonyBot.BotSay(site, username + ", нельзя ставить больше " + param["Maximum"] + " кредитов!");
                }    
                else
                {
                    RutonyBot.BotSay(site, username + ", you can not bet more than  " + param["Maximum"] + " credits!");
                }
                return;
            }
			
			
			cr.CreditsQty -= credit;
            
			string stavka = arrString[2].Trim().ToLower();
			if (stavka != "красное" && stavka != "черное" && stavka != "ноль" && stavka != "red" && stavka != "black" && stavka != "zero")
			{
                if (lang == 0)
                {
                    RutonyBot.BotSay(site, username + " вам нужно указать, на что ставите (красное/черное/ноль)");
                }    
                else
                {
                    RutonyBot.BotSay(site, username + " you need to specify what to bet (red/black/zero)");
                }
                return;
			}	
			
			Random rnd = new Random();
            int randomShoot = rnd.Next(1, 40);

            if (randomShoot <= 18) {
				if (stavka == "красное" || stavka == "red")
				{
					status = 1;
					cr.CreditsQty += credit*2;
				}
            } else if (randomShoot > 18 && randomShoot <= 38) {
				if (stavka == "черное" || stavka == "black" )
				{
					status = 1;
					cr.CreditsQty += credit*2;
				}
            } else {
				if (stavka == "ноль" || stavka == "zero" )
				{
					status = 2;
					cr.CreditsQty += credit*5;
				}
            }
            RankControl.ChatterRank cr_change = RankControl.ListChatters.Find(r => r.Nickname == username.Trim().ToLower());
            switch (status) {
                case 0:
                    if (lang == 0)
                    {
                        RutonyBot.BotSay(site, username + " вы проиграли " + credit + " кредитов. Теперь у вас " + cr_change.CreditsQty + " кредитов");
                    }    
                    else
                    {
                        RutonyBot.BotSay(site, username + " you lost " + credit + " credits. Now you have " + cr_change.CreditsQty + " credits");
                    }
                    break;
                case 1:
                    if (lang == 0)
                    {
                        RutonyBot.BotSay(site, username + " вы выиграли " + credit*2 + " кредитов. Теперь у вас " + cr_change.CreditsQty + " кредитов");
                    }    
                    else
                    {
                        RutonyBot.BotSay(site, username + " you won " + credit*2 + " credits. Now you have " + cr_change.CreditsQty + " credits");
                    }
                    break;
                case 2:
                    if (lang == 0)
                    {
                        RutonyBot.BotSay(site, username + " вы выиграли " + credit*5 + " кредитов, поставив на ноль! Теперь у вас " + cr_change.CreditsQty + " кредитов");
                    }    
                    else
                    {
                        RutonyBot.BotSay(site, username + " you won " + credit*5 + " credits, having bet on a zero! Now you have " + cr_change.CreditsQty + " credits");
                    }
                    break;
            }


        }
    }
    class CreditName
	{

		public string[] edpads {get; set;}
		public string[] mnpads {get; set;}

		public CreditName()
		{
			edpads = new string[6];
			mnpads = new string[6];
		}




	}
}
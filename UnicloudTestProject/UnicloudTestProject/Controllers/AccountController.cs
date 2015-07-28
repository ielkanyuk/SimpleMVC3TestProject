using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using UnicloudTestProject.Models;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace UnicloudTestProject.Controllers
{
    public class AccountController : Controller
    {         
        public class EmailManager
        {
            private const string EmailFrom = "noreplay@gmail.com";

            /// <summary>
            /// Отправление письма для подтверждения регистрации
            /// </summary>
            /// <param name="userName">Логин пользователя</param>
            public static void SendConfirmationEmail(string userName)
            {
                var user = Membership.GetUser(userName.ToString());
                var confirmationGuid = user.ProviderUserKey.ToString();
                var verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/Verify/" + confirmationGuid;
 
                using (var client = new SmtpClient())
                {
                    using (var message = new MailMessage(EmailFrom, user.Email))
                    {
                        message.Subject = "Подтверждение регистрации Unicloud: Тестовое задание";
                        message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Вы зарегистрировались в системе Unicloud: Тестовое задание." +
                                       "</p><p>Для завершения регистрации необходимо перейти по ссылке:</p>" + 
                                       "<p><a href=\"" + verifyUrl + "\" target=\"_blank\">" + verifyUrl + 
                                       "</a></p><div>С наилучшими пожеланиями,</div><div> система Unicloud: Тестовое задание</div><p>Письмо сформировано роботом, пожалуйста, не отвечайте на него.</p></body></html>";
 
                        message.IsBodyHtml = true;
                    
                        client.EnableSsl = true;
                        client.Send(message);
                    };
                };
            }
        } 

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Подтверждение регистрации
        /// </summary>
        /// <param name="id">ID пользователя</param>        
        [HttpGet]
        public ActionResult Verify(string id)
        {
            if (string.IsNullOrEmpty(id) || (!Regex.IsMatch(id, @"[0-9a-f]{8}\-([0-9a-f]{4}\-){3}[0-9a-f]{12}")))
            {
                ViewBag.Msg = "Некорректный ID";
                return View();
            }

            else
            {
                var user = Membership.GetUser(new Guid(id));

                if (!user.IsApproved)
                {
                    user.IsApproved = true;
                    Membership.UpdateUser(user);
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {                    
                    ViewBag.Msg = "Регистрация уже была подтверждена";
                    return View();
                }
            }
        } 

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, false, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    EmailManager.SendConfirmationEmail(model.UserName);
                    return RedirectToAction("RegisterConfirmation", "Account");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Пользователь с таким именем уже зарегистрирован в системе";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

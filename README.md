# Mailzory
Mailzory helps you to send emails which are based on Razor templates. Mailzory is dependent on RazorEngine project for compiling your Razor templates and that means you are limited to the power of RazorEngine for template processing.

## Install (Nuget)

```powershell
Install-Package Mailzory

```
## Usage

how will Mailzory helps us to send email, if we have the following template?


```html
<div>

    <h1 style="font-size:13px">
        <label>@ViewBag.Name</label>
    </h1>

    <hr>

    <div>
        <p>
            @ViewBag.Content
        </p>       
    </div>


</div>
```

* Sendig email with default configurations

Mailzory uses SmtpClient for sending emails, but as you can see there is no sign of SmptClient in the following snippet. In this case Mailzory uses [mailSettings](https://msdn.microsoft.com/en-us/library/w355a94k(v=vs.110).aspx) in your web.config (or app.config) for configuring the SmtpClient.

```c#
// template path
var viewPath = Path.Combine(HostingEnvironment.MapPath("~/Views/Emails"), "hello.cshtml"); 
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);
var email = new Email(template);
// set ViewBag properties
email.ViewBag.Name = "Johnny";
email.ViewBag.Content = "Mailzory Is Funny";
// send it
email.Send("mailzory@mailzory.co", "subject");
```

A sample for mailSettings at web.config (or app.config)

```xml

  <system.net>
    <mailSettings>
      <smtp deliveryMethod="Network" from="{the mail address which is sending your emails: mailzor@isgood.com}">
        <network host="{mail server address}" port="{mail server port}" defaultCredentials="{true|false}" userName="{username}" password="{password}" />
      </smtp>
    </mailSettings>
  </system.net>
  
```

# Mailzory
Mailzory helps you to send emails which are based on Razor templates. Mailzory is dependent on RazorEngine project for compiling your Razor templates and that means you are limited to the power of RazorEngine for template processing.

## Install (Nuget)

```powershell
Install-Package Mailzory

```
## Usage

So how will Mailzory helps us to send emails?

Suppose we have the following loosely typed template. its name is hello.cshtml

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

And also suppose that we have the following strongly typed template. its name is typedHello.cshtml

```html

@model Test.MessageModel
<div>

    <h1 style="font-size:13px">
        <label>@Model.Name</label>
    </h1>

    <hr>

    <div>
        <p>
            @Model.Content
        </p>
    </div>

</div>

```

* Sendnig syncronous email with default configurations (Loosely Typed Template)

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
        <network enableSsl="{true|false}" host="{mail server address}" port="{mail server port}" defaultCredentials="{true|false}" userName="{username}" password="{password}" />
      </smtp>
    </mailSettings>
  </system.net>
  
```

* Sendnig asyncronous email with default configurations (Loosely Typed Template)

You can send emails asynchronously. Mailzory async methods are return a Task instance.

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
var task = email.Send("mailzory@mailzory.co", "subject");
task.Wait();
```

* Sending email (Strongly Typed Templates)

In addition to ViewBag, you can pass a strongly typed model to your template. Note that in the following example we are sending an email for multipe recievers.

```c#
var viewPath = Path.Combine("Views/Emails", "typedHello.cshtml");
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);
// fill model
var model = new MessageModel
{
    Content = "Mailzory Is Funny. Its a Model Based message.",
    Name = "Johnny"
};

var email = new Email<MessageModel>(template,model);
// send it
var task =
    email.SendAsync(new[] { "mailzory1@mailzory.co","mailzory2@mailzory.co" }
    , "subject");

task.Wait();
```

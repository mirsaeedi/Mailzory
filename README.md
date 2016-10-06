# Mailzor
Mailzor helps you to send emails which are based on Razor templates. Mailzor is dependent on RazorEngine project for compiling your Razor templates and that means you are limited to the power of RazorEngine for template processing.

## Install

```
$ npm install --save pageres
```
## Usage

how will Mailzor helps us to send email, if we have the following template?


```razor
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

Sendig email with default configurations.

```c#
// template path
var viewPath = Path.Combine(HostingEnvironment.MapPath("~/Views/Emails"), "hello.cshtml"); 
// read the content of template and pass it to the Email constructor
var email = new Email(File.ReadAllText(viewPath));
// set ViewBag properties
email.ViewBag.Name = "Johnny";
email.ViewBag.Content = "Mailzor Is Funny";
// send it
email.Send(issueResponsible.User.Email, "subject");                
```

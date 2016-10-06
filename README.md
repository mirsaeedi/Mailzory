# Mailzor
Mailzor helps you to send emails which are based on Razor templates. Mailzor is dependent on RazorEngine project for compiling your Razor templates and that means are limited to the power of RazorEngine in processing templates.

## Install

```
$ npm install --save pageres
```
## Usage

```js
const Pageres = require('pageres');

const pageres = new Pageres({delay: 2})
	.src('yeoman.io', ['480x320', '1024x768', 'iphone 5s'], {crop: true})
	.src('todomvc.com', ['1280x1024', '1920x1080'])
	.src('data:text/html;base64,PGgxPkZPTzwvaDE+', ['1024x768'])
	.dest(__dirname)
	.run()
	.then(() => console.log('done'));
```

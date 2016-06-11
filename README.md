Scriptomatic
========

Scriptomatic is an automated UI framework based on TestStack.White. The API follows a safe and chainable design that can be used with either C#, JavaScript, or CoffeeScript.

__Note:__ _Scriptomatic is still in its early stages of development._

Examples
-----------

Automating the calculator:

```coffeescript
console.log(
    desktop
        .showDesktop()
        .run('calc.exe')
        .wait(1000)
        .windowsByName('Calculator')
            .restore()
            .findAndClick(['Clear'])
            .findAndClick(['1', '2', '3'])
            .findAndClick(['Add'])
            .findAndClick(['4', '5', '6'])
            .findAndClick(['Divide'])
            .findAndClick(['2'])
            .findAndClick(['Equals'])
            .labels()
                .get(3)
                .value()
)
```

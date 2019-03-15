![](imgs/enigma.png)

# enigma.net
[![CodeFactor](https://www.codefactor.io/repository/github/q2g/enigma.net/badge)](https://www.codefactor.io/repository/github/q2g/enigma.net)
[![travis](https://travis-ci.com/q2g/enigma.net.svg?branch=master)](https://travis-ci.com/q2g/enigma.net/)
[![CLA assistant](https://cla-assistant.io/readme/badge/q2g/enigma.net)](https://cla-assistant.io/q2g/enigma.net)
[![Nuget](https://img.shields.io/nuget/v/enigma.net.svg)](https://www.nuget.org/packages/enigma.net)

enigma.net is a library that helps you communicate with a Qlik Associative Engine.
Examples of use may be building your own analytics tools, back-end services, or other tools communicating with a Qlik Associative Engine. As an example Qlik Core provides an easy way to get started and in the tests.
enigma.net is compared to the Qlik.NET SDK .NET Core ready, but also support the regular .NET Framework.
For more details see the comparison.

---

- [Getting started](docs/getting-started)
- [Qlik Core](https://core.qlik.com/)
- [Runnable examples](./examples/README.md)
- [Schema Github](https://github.com/q2g/qlik-engineapi) [![Schema nuget](https://img.shields.io/nuget/v/qlik-engineapi.svg)](https://www.nuget.org/packages/qlik-engineapi)

---

## Installation

```net
dotnet add package enigma.net
```

## Getting started

cooming soon

## Schemas

enigma.net does't includes generated API code. The shema is available as extra nuget package qlik-engineapi.
The intention is to separate the shema complete from this library, so that you are free which versions you combine.
[Schema Github](https://github.com/q2g/qlik-engineapi)

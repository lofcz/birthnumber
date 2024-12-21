[![LlmTornado](https://badgen.net/nuget/v/LibBirthnumber?v=302&icon=nuget&label=LibBirthnumber)](https://www.nuget.org/packages/LibBirthnumber)

# LibBirthnumber

Simple validator of National Identification Number.

## ⚡Getting Started
```
Install-Package LibBirthnumber
```

## Validate
```cs
bool result = BirthnumberCs.TryParse("825611/5769", out BirthnumberCs? birthNumber);
```

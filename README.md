<img src="https://docs.letportal.app/assets/images/logo.png" width="200"/>

# LET Portal

## Version: 0.9.0
![Forks](https://img.shields.io/github/forks/phucan1108/letportal?style=plastic)

![License](https://img.shields.io/github/license/phucan1108/letportal?style=plastic)

![Build](https://github.com/phucan1108/letportal/workflows/LET%20Portal%20Build/badge.svg)

![Publish](https://github.com/phucan1108/letportal/workflows/Publish%20LETPortal.Core%20Lib/badge.svg)

LET Portal is the rapid web platform for developers. It helps turn the data into the form, the gridview, the chart, the tree and combine these components to one page. The project is open-source project. There are no fee charged to use or modify.

The current version is **0.9.0**. We are trying to push quickly many changes so you can help us by logging many issues which you found, that's great.

# Visit our homepage

You can reach this [Home page](https://letportal.app) to keep up-to-date features.

<img src="https://docs.letportal.app/assets/images/getting-started/homescreen.jpg" width="600"/>
                                                                                          
<img src="https://letportal.app/images/letportal/featurespage/header-banner.png" width="600" />          

# Technologies

LET Portal is built by using SPA architecture. That means I am using .NET Core Web API as Back end service and Angular as Front end page. These databases below are using to install and query. Docker helps you to run quickly on Windows or Linux (Ubuntu :smiley:)

- :white_check_mark: .NET Core 3.1
- :white_check_mark: Angular 9, Material Angular
- :white_check_mark: Docker
- :white_check_mark: MongoDB 4
- :white_check_mark: SQL Server 2012+
- :white_check_mark: MySQL
- :white_check_mark: PostgreSQL

# High Level Design

![High Level Design](https://docs.letportal.app/assets/images/overview/architecture.png)

According to an architecture above, LET Portal has {==**two components and one 3rd-party**==}. There are:

- SPA Web: **Angular 9**, our main front-end web application. It will connect to Saturn to perform APIs.
- Saturn: **.NET Core 3.1**, our main back-bone web service. It will provide four big features: Identity, Portal, Chat&Video, Microservices
- Proxy Server: [Nginx](https://www.nginx.com/)

# Saturn

![Saturn Components](https://docs.letportal.app/assets/images/overview/saturn-components.png)

In the architecture above, Saturn consists of four main features: Identity, Portal, Chat&Video and Microservice. 

- **Identity and Portal:** work with Web API, that mean they expose the public endpoint to be called.
- **Chat&Video:** works under SignalR, that mean they expose the real-time connection between Client and Server.
- **Microservice:** work under gRPC, that mean they only expose the private endpoint in HTTP/2 protocol to be called via inter-service communication.

# Highlight Features

## Standard Form

Quickly create the form by scanning the data from the database. Ex: If you have a SELECT clause, you want to convert it into the form. Just paste it into Standard Builder and automatically create the Form!

- [x] Support MongoDB, SQL Server, MySQL and PostgreSQL to transfrom the form
- [x] 15+ built-in controls such as Textbox, Textarea, Rich Text Editor
- [x] Select/Autocomplete with configurable datasource, multiple choices
- [x] Support synchronous and asynchronous Validation
- [x] Support Localization

<img src="https://letportal.app/images/letportal/featurespage/standard-banner.png" width="600"/>

## Array

Quickly create the gridview which can manipulate array data. Ex: If you have a SELECT clause with returning multiple records, you want to convert it into the gridview to manipulate it. Just paste it into Standard Builder and automatically create the GridView!

- [x] Observe CUD in a list
- [x] Detect dirty object
- [x] Allow to separate INSERT, UPDATE and DELETE command when updating to Database.

<img src="https://letportal.app/images/letportal/featurespage/array-banner.png" width="600"/>

## Tree

Quickly create the tree which can manipulate tree data. Ex: You have a nested data such as **menu**, you want to canvas it with Builder!

- [x] Data can be nested or flat
- [x] Support to convert input and output between nested and flat
- [x] UI/UX following to Material Design

<img src="https://letportal.app/images/letportal/featurespage/tree-banner.png" width="600"/>

## Dynamic List

Quickly create an advanced search list which can search/sort/filter the data.

- [x] Support MongoDB, SQL Server, MySQL and PostgreSQL
- [x] Support Advanced Filter with many control types
- [x] Support format the data as HTML
- [x] Export to CSV (in client only)

<img src="https://letportal.app/images/letportal/featurespage/list-banner.jpg" width="600"/>

## Chart

Quickly create the chart by converting the query into chart. Support many chart types such as Pie Chart, Bar Chart, etc.

- [x] Support vary chart type
- [x] Support Advanced filter
- [x] Support real-time data

<img src="https://letportal.app/images/letportal/featurespage/chart-banner.jpg" width="600"/>

## Chat & Video Call

Allow user to communicate via LET Portal, save file within the system and make a video call with WebRTC.

<img src="https://letportal.app/images/letportal/featurespage/chat-banner.png" width="600"/>

## Mobile Ready

All components are built to be mobile-friendly.

<img src="https://letportal.app/images/letportal/featurespage/mobile-banner.png" width="300"/>


## More and more features

We also provide many simple but elegant feature such as Chat, Video Call, CLI. Also we provide a minor Microservice solution to help small team can start the development.

# Getting Started

On Windows OS, you can visit [this page](https://letportal.app/getting-started/windows)

On Linux Ubuntu OS, you can visit [this page](https://letportal.app/getting-started/linux)

# Documentation

You can read here [Documentation](https://docs.letportal.app) for full documentation

# Important note

If you want to roll out LET Portal to Production, please contact me to get more detail.

# Code Quality

![Result](https://codescene.io/projects/7362/status.svg)

Seems I have a lot of things to do which this hotspot :satisfied:

[![CodeScene Code Health](https://codescene.io/projects/7362/status-badges/code-health)](https://codescene.io/projects/7362)

[![CodeScene System Mastery](https://codescene.io/projects/7362/status-badges/system-mastery)](https://codescene.io/projects/7362)

[![CodeScene general](https://codescene.io/images/analyzed-by-codescene-badge.svg)](https://codescene.io/projects/7362)

# Licenses

LET Portal typically use [MIT](LICENSE).

Copyrigh@2020 An Quang Phuc Le.
Email support: letportal2020@gmail.com


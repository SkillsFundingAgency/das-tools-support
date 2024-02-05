# das-tools-support

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-tools-support?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2222&branchName=master) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-tools-support&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-tools-support)


The application contains the following functionality:

* Bulk Stop Apprenticeships
* Bulk Pause Apprenticeships
* Bulk Resume Apprenticeships
* Bulk Suspend Users
* Bulk Reinstate Users

## Getting Started

To get the project running you will need to:

*  Clone a copy of [the das-tools-support repository](https://github.com/SkillsFundingAgency/das-tools-support.git)
*  Set the start up project to SFA.DAS.Tools.Support.Web
*  Restore packages
*  The [Commitments API](https://github.com/SkillsFundingAgency/das-commitments) either running locally or in an accessible test environment
*  The latest configuration from the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) in your Azure Table Storage Emulator (This can be added using the [das-employer-config-updater repository](https://github.com/SkillsFundingAgency/das-employer-config-updater))

> Both the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) and the [das-employer-config-updater repository](https://github.com/SkillsFundingAgency/das-employer-config-updater) are private and only available to members of the SkillsFundingAgency GitHub Organisation

### Prerequisites

* An IDE supporting .Net6
* A valid Staff IDAMS account
* The stop and pause/resume roles added to your IDAMS account
* The [Commitments API](https://github.com/SkillsFundingAgency/das-commitments)
* The latest configuration from [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config)
* Azure Table Storage Emulator

## Usage

* Run the application
* Select "Open" next to the "Support Tool" option in the menu
* Select "Open" next to the "Stop an apprenticeship" option
* Enter search criteria and click "Submit"
* Any matching apprenticeships will be returned and can be stopped using the UI

## Testing

* Unit tests are available in the SFA.DAS.Tools.Support.UnitTests project and can be run using any xUnit test runner

## Known Issues

* No known issues

## License
Licensed under the [MIT License](https://github.com/SkillsFundingAgency/das-tools-support/blob/master/LICENSE)

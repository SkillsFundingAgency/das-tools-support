FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

ENV PROJECT_PATH=SFA.DAS.Tools.Support.Web/SFA.DAS.Tools.Support.Web.csproj
ENV SLN_PATH=das-tools-support\src\SFA.DAS.Tools.Support.Web.sln
COPY ./src ./src
WORKDIR /src

RUN dotnet restore
RUN dotnet build -c release --no-restore
RUN dotnet test -c Release --no-restore --no-build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

ENV PROJECT_PATH=SFA.DAS.Tools.Support.Web/SFA.DAS.Tools.Support.Web.csproj
ENV TEST_PATH=SFA.DAS.Tools.Support.UnitTests/SFA.DAS.Tools.Support.UnitTests.csproj
COPY ./src ./src
WORKDIR /src

RUN dotnet restore
RUN dotnet build -c release --no-restore
RUN dotnet test -c Release --no-restore --no-build
RUN dotnet publish $PROJECT_PATH -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SFA.DAS.Tools.Support.Web.dll"]

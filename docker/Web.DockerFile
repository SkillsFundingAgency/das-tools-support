FROM das-tools-support-build:latest AS build
WORKDIR /src

ENV PROJECT_PATH=SFA.DAS.Tools.Support.Web/SFA.DAS.Tools.Support.Web.csproj
RUN dotnet publish $PROJECT_PATH -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SFA.DAS.Tools.Support.Web.dll"]

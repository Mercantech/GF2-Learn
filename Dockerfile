FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY GF2Learn.slnx ./
COPY src/GF2Learn.Web/GF2Learn.Web.csproj src/GF2Learn.Web/
COPY src/GF2Learn.Web.Client/GF2Learn.Web.Client.csproj src/GF2Learn.Web.Client/
RUN dotnet restore src/GF2Learn.Web/GF2Learn.Web.csproj
COPY content/ content/
COPY src/GF2Learn.Web/ src/GF2Learn.Web/
COPY src/GF2Learn.Web.Client/ src/GF2Learn.Web.Client/
WORKDIR /src/src/GF2Learn.Web
RUN dotnet publish -c Release -o /app/publish /p:SkipPlaygroundBclCopy=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/content ./content
ENV ASPNETCORE_URLS=http://+:8080
ENV PathBase=
ENV ContentPath=/app/content
EXPOSE 8080
ENTRYPOINT ["dotnet", "GF2Learn.Web.dll"]

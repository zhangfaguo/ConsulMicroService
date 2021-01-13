#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/dotnetcn/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM registry.cn-hangzhou.aliyuncs.com/zjueva/dotnet-sdk-5.0:0.0.1 AS build
WORKDIR /src
COPY ["ApiGetWay/Consul.MicroServer.ApiGetway/Consul.MicroServer.ApiGetway.csproj", "ApiGetWay/Consul.MicroServer.ApiGetway/"]
RUN dotnet restore "ApiGetWay/Consul.MicroServer.ApiGetway/Consul.MicroServer.ApiGetway.csproj"
COPY . .
WORKDIR "/src/ApiGetWay/Consul.MicroServer.ApiGetway"
RUN dotnet build "Consul.MicroServer.ApiGetway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consul.MicroServer.ApiGetway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consul.MicroServer.ApiGetway.dll"]

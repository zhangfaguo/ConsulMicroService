#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/dotnetcn/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM registry.cn-hangzhou.aliyuncs.com/zjueva/dotnet-sdk-5.0:0.0.1 AS build
WORKDIR /src
COPY ["Clients/Consul.MicroServer.Client/Consul.MicroServer.Client.csproj", "Clients/Consul.MicroServer.Client/"]
COPY ["MicroService/UserService/Consul.MicroService.UserService.Interfaces/Consul.MicroService.UserService.Interfaces.csproj", "MicroService/UserService/Consul.MicroService.UserService.Interfaces/"]
COPY ["Cores/Zfg.Consul/Zfg.Consul.csproj", "Cores/Zfg.Consul/"]
RUN dotnet restore "Clients/Consul.MicroServer.Client/Consul.MicroServer.Client.csproj"
COPY . .
WORKDIR "/src/Clients/Consul.MicroServer.Client"
RUN dotnet build "Consul.MicroServer.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consul.MicroServer.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consul.MicroServer.Client.dll"]
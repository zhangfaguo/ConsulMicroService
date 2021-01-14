#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/dotnetcn/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM registry.cn-hangzhou.aliyuncs.com/zjueva/dotnet-sdk-5.0:0.0.1 AS build
WORKDIR /src
COPY ["MicroService/UserService/Consul.MicroService.UserService/Consul.MicroService.UserService.csproj", "MicroService/UserService/Consul.MicroService.UserService/"]
COPY ["MicroService/UserService/Consul.MicroService.UserService.AppService.Contract/Consul.MicroService.UserService.AppService.Contract.csproj", "MicroService/UserService/Consul.MicroService.UserService.AppService.Contract/"]
COPY ["Cores/Zfg.Libraries/Zfg.Libraries.csproj", "Cores/Zfg.Libraries/"]
COPY ["Cores/Zfg.Core.Application/Zfg.Core.Application.csproj", "Cores/Zfg.Core.Application/"]
COPY ["Cores/Zfg.Core/Zfg.Core.csproj", "Cores/Zfg.Core/"]
COPY ["MicroService/UserService/Consul.MicroService.UserService.AppService/Consul.MicroService.UserService.AppService.csproj", "MicroService/UserService/Consul.MicroService.UserService.AppService/"]
COPY ["MicroService/UserService/Consul.MicroService.UserService.Domain/Consul.MicroService.UserService.Domain.csproj", "MicroService/UserService/Consul.MicroService.UserService.Domain/"]
COPY ["Cores/Zfg.Core.Db/Zfg.Core.Db.csproj", "Cores/Zfg.Core.Db/"]
COPY ["Cores/Zfg.Consul/Zfg.Consul.csproj", "Cores/Zfg.Consul/"]
RUN dotnet restore "MicroService/UserService/Consul.MicroService.UserService/Consul.MicroService.UserService.csproj"
COPY . .
WORKDIR "/src/MicroService/UserService/Consul.MicroService.UserService"
RUN dotnet build "Consul.MicroService.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consul.MicroService.UserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consul.MicroService.UserService.dll"]
CMD ["--urls","http://*:80"]
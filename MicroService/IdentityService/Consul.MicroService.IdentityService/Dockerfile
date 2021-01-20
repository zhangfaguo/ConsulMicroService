#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/dotnetcn/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM registry.cn-hangzhou.aliyuncs.com/zjueva/dotnet-sdk-5.0:0.0.1 AS build
WORKDIR /src
COPY ["MicroService/IdentityService/Consul.MicroService.IdentityService/Consul.MicroService.IdentityService.csproj", "MicroService/IdentityService/Consul.MicroService.IdentityService/"]
RUN dotnet restore "MicroService/IdentityService/Consul.MicroService.IdentityService/Consul.MicroService.IdentityService.csproj"
COPY . .
WORKDIR "/src/MicroService/IdentityService/Consul.MicroService.IdentityService"
RUN dotnet build "Consul.MicroService.IdentityService.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Consul.MicroService.IdentityService.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consul.MicroService.IdentityService.dll"]
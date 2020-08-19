FROM microsoft/dotnet:3.0-sdk as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY Test.csproj /build/

RUN dotnet restore ./build/Test.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./Test.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM microsoft/dotnet:3.0-aspnetcore-runtime
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "Test.dll"] 
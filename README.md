# Pong Game Server for C#

이 프로젝트는 iFun Engine을 사용하는 Unity3d 사용자를 위한 샘플 게임 서버입니다. 해당 게임 서버를 테스트하기 위한 환경은 다음과 같습니다.

* Ubuntu 16.04 혹은 18.04, CentOS 7
* mysql 혹은 mariadb
* zookeeper
* funapi-leaderboard

**해당 문서에서는 Ubuntu 16.04에서 설정하는 방법을 서술합니다.**

## 목차

* [다운로드](#다운로드)
* [서버 환경 설정](#서버-환경-설정)
    - [mysql 설치](#mysql-설치)
    - [mysql 설치 후 환경설정](#mysql-설치-후-환경설정)
    - [zookeeper 설치](#zookeeper-설치)
    - [funapi-leaderboard 설치](#funapi-leaderboard-설치)
    - [funapi-leaderboard 설치 후 환경설정](#funapi-leaderboard-설치-후-환경설정)
* [프로젝트 디렉터리 구조](#프로젝트-디렉터리-구조)
* [게임 서버 빌드](#게임-서버-빌드)
* [게임 서버 실행](#게임-서버-실행)
* [테스트](#테스트)

## 다운로드

**Pong Server** 프로젝트를 **git clone** 으로 다운 받거나 **zip 파일** 을 다운 받아 주세요.

```bash
git clone https://github.com/iFunFactory/game-pong-server-csharp.git
```

## 서버 환경 설정

Pong-server를 구동하기 위해 `mysql`, `zookeeper`, `funapi-leaderboard`의 설치, 환경설정이 필요합니다. 각 개발환경 설정에 대한 자세한 내용은 [튜토리얼](https://www.ifunfactory.com/engine/documents/tutorial/ko/project.html#object-relational-mapping-db)과 [메뉴얼](https://www.ifunfactory.com/engine/documents/reference/ko/development-environment.html)을 참고해 주세요.

#### mysql 설치
`mysql server`가 설치되어있지 않다면 아래의 명령어를 통해 mysql server를 설치해주세요.

```bash
$ sudo apt-get install mysql-server
```

#### mysql 설치 후 환경설정

```bash
$ mysql -u root -p

mysql> create user 'funapi'@'localhost' identified by 'funapi';

mysql> grant all privileges on *.* to 'funapi'@'localhost';

mysql> create database funapi;

mysql> create database funapi_leaderboard;

$ sudo service mysql start
```

#### zookeeper 설치

```bash
$ sudo apt-get install zookeeper zookeeperd
$ sudo service zookeeper start
```

#### funapi-leaderboard 설치

leaderboard는 agent구조로 되어있습니다. 아래의 명령어를 이용해 설치합니다.

```bash
$ sudo apt-get update
$ sudo apt-get install funapi-leaderboard1
```

또한 리더보드는 캐시 처리를 위해 `Redis` 를 사용합니다. 아래 명령어를 이용하여 Redis를 설치합니다.

```bash
$ sudo apt-get install redis-server
```

#### funapi-leaderboard 설치 후 환경설정

리더보드 에이전트가 정상적으로 설치되었다면 `/usr/share/funapi-leaderboard/default/manifests/MANIFEST.json` 설정파일이 생성됩니다.

`MANIFEST.json` 파일을 열고, 아래와 같이 변경해주세요.

```bash
$ sudo vim /usr/share/funapi-leaderboard/default/manifests/MANIFEST.json
```

```json
{
  ...
  "components": [
    {
      ...
      "dependency": {
        ...
        "Redis": {
          "enable_redis": true,
          "redis_mode": "redis",
          "redis_servers": {
            "": {
              "address": "127.0.0.1:6379",
              "auth_pass": ""
            }
          },
          "redis_async_threads_size": 10
        }
      },
      "arguments": {
        "server_tcp_port": 12820,
        "mysql_server_url": "tcp://127.0.0.1:3306",
        "mysql_id": "funapi",
        "mysql_pw": "funapi",
        "mysql_db_name": "funapi_leaderboard",
        "mysql_db_connection_count": 10,
        "reset_schedules": [
          {
            "leaderboard_id": "winning_streak",
            "period": "day",
            "interval": 1,
            "starts": "2015-01-01 05:00:00",
            "ends": "2020-12-31 23:00:00"
          },
          {
            "leaderboard_id": "winning_streak",
            "period": "week",
            "interval": 1,
            "starts": "2015-01-01 05:00:00",
            "ends": "2020-12-31 23:00:00"
          }
        ]
      },
      "library": "libfunapi-leaderboard.so"
    }
  ]
}
```

* `leaderboard_id` : 랭킹을 구분하기 위한 이름으로 winning_streak 를 사용합니다. 연승 랭킹입니다.
* `period` : 랭킹 리셋 주기를 의미합니다.
* `interval` : 랭킹을 리셋하는 주기의 간격을 입력합니다.
* `start` : 랭킹을 리셋하는 스케쥴의 최초 시작 날짜와 시간을 입력합니다.
* `ends` : 랭킹을 리셋하는 스케쥴의 종료 날짜와 시간을 입력합니다.

설정이 완료되면 다음 명령어를 통해 실행합니다.

```bash
$ sudo service funapi-leaderboard start
```

`MANIFEST.json` 의 각 Argument들에 대한 자세한 정보는 관련 [메뉴얼](https://www.ifunfactory.com/engine/documents/reference/ko/leaderboard-service.html#leaderboard-service-agent-configuration)을 참고해주세요.

## 프로젝트 디렉터리 구조
다운로드와 서버 환경 설정이 완료되었다면 `game-pong-server-csharp/pongcs-source` 디렉터리로 가봅시다. source 디렉터리에는 여러 파일들이 있는데, 각각을 간략히 설명하자면 다음과 같습니다. 자세한 내용은 [메뉴얼](https://www.ifunfactory.com/engine/documents/reference/ko/development-environment.html#source)을 참고해 주세요.

* `VERSION`: 생성되는 게임의 버전 숫자를 기록합니다.
* `DEBIAN`: 생성되는 게임을 DEB 패키지 형태로 묶는 경우 DEB 패키지 생성에 필요한 내용들을 기술합니다.
* `LICENSE`: 배포시에 적용할 사용권 등을 설명하실 수 있습니다.
* `README`: 내부적으로 게임 소스에 대한 설명 글을 남겨야될 때 이 파일을 이용하시면 됩니다.
* `setup_build_environment`: source 디렉터리를 이용해서 빌드 디렉터리를 생성할 때 실행될 스크립트입니다.
* `CMakeLists.txt`: 프로젝트의 최상위 레벨 cmake 입력 파일입니다.

* **`src/`**: C++ 을 위한 코드로 여기서는 쓰이지 않습니다. 무시합니다.

* **`mono/`**: C# 프로젝트 파일, 소스코드들이 위치합니다.

* `mono/pongcs.csproj`: 프로젝트 파일입니다.

Monodevelop 등의 IDE 프로그램으로 `mono/pongcs.csproj` 를 열었을 때 보여지는 파일에 대한 설명입니다.

* `ActivityLog/pongcs_loggers.json`: Activity Log 를 정의하는 파일입니다.
* `ActivityLog/pongcs_loggers.cs`: (빌드시 자동생성) pongcs_loggers.json 의 정의에 따라 Activity Log API 가 만들어집니다.
* `Manifest/MANIFEST.lobby.json`: Lobby 서버를 설정하는 파일입니다.
* `Manifest/MANIFEST.game.json`: Game 서버를 설정하는 파일입니다.
* `Manifest/MANIFEST.matchmaker.json`: Matchmaker 서버를 설정하는 파일입니다.
* `ObjectModel/example.json`: iFunEngine ORM 의 Object 를 정의하는 파일입니다.
* `ObjectModel/pongcs_object.cs`: (빌드시 자동생성) example.json 의 정의에 따라 ORM API 가 만들어집니다.
* `Protobuf/`: Google Protocol Buffers 관련 파일들입니다. 여기서는 쓰이지 않습니다. 무시합니다.
* `common.cs`: Lobby, Game 서버 공통의 핸들러 파일이 위치합니다.
* `server.cs`: Lobby, Game, Matchmaker 공통으로 서버가 시작될 때 초기화를 진행하는 파일입니다.
* `lobby_server.cs`: Lobby 서버를 구현하는 파일입니다.
* `game_server.cs`: Game 서버를 구현하는 파일입니다.
* `matchmaking_server.cs`: Matchmaker 서버를 구현하는 파일입니다.
* `leaderbaord.cs`: 랭킹 처리를 위한 파일입니다.
* `utility.cs`: 유틸리티 함수들을 위한 파일입니다.


## 게임 서버 빌드

(1.0.0-4358 Experimental 버전에서 테스트 되었습니다. 아이펀 엔진 레퍼런스 문서의 [배포판 타입 선택하기](https://www.ifunfactory.com/engine/documents/reference/ko/install.html#select-funapi-repo) 의 설명을 참고하여 엔진을 업데이트 하시기 바랍니다.)

게임 서버를 빌드하는 방법은 크게 2 가지입니다. Monodevelop 등의 IDE 를 이용하는 방법과 Terminal 에서 직접 빌드하는 방법입니다.
코딩을 하며 빌드할 때는 IDE 를 이용하는 것이 좋으며, Lobby, Game, Matchmaker 서버를 모두 실행하여 테스트 할 때는 Terminal 에서 빌드하는 것이 좋습니다.

### Monodevelop 으로 빌드하기

1. Monodevelop 에서 `game-pong-server-csharp/pongcs-source/mono/pongcs.csproj` 열기
2. 메뉴의 Build -> Build All(F8) 로 빌드하기

### Terminal 에서 빌드하기

1. 빌드 환경 만들기

    ```bash
    pongcs-source/setup_build_environment --type=makefile
    ```
    명령어를 실행시키면 빌드 환경 생성이 진행됨을 알리는 로그가 출력됩니다. 빌드 환경 생성이 완료되면 프로젝트 최상위 디렉터리에 `pongcs-build` 폴더가 새롭게 생성됩니다. `pongcs-build` 디렉터리 하위에는 `debug`와 `release` 디렉터리가 생성되는데, 각각의 디렉터리에서 debug버전 빌드와 release버전의 빌드를 할 수 있습니다.

2. 빌드하기

    `pongcs-build/debug` 디렉터리로 이동하여 `make`명령을 입력해 봅시다. 다음과 같은 메시지가 출력되었다면 성공입니다.

    ```bash
    $ cd pongcs-build/debug
    $ make

    ...
    Linking CXX shard module libpongcs.so
    [100%] Built target Pong
    ```

iFunEngine은 빌드 후 `-local` 스크립트와 `-launcher` 스크립트를 생성합니다. `-local` 스크립트는 개발 중에 서버를 실행할 때 사용하며 `-launcher` 스크립트는 게임 서버를 패키징하여 데몬으로 실행할 때 사용합니다. 또한, 해당 프로젝트에서는 flavor 기능을 사용하여 lobby, matchmaker, game으로 나누어 서버를 관리하고 있습니다. 때문에 빌드가 완료되면 lobby, matchmaker, game서버마다 별도로 `-local`스크립트와 `-launcher`스크립트가 생성됩니다.

* `lobby server` : 로그인 처리를 하며 matchmaking 대기 중인 클라이언트가 머무르는 서버입니다.
* `matchmaker server` : matchmaking 을 처리하는 서버입니다.
* `game server` : 매칭된 클라이언트가 머무르는 서버입니다.

flavor에 대한 자세한 내용은 [메뉴얼](https://www.ifunfactory.com/engine/documents/reference/ko/mgmt-packaging.html#flavor)을 참고해 주세요.

## 게임 서버 실행

서버를 실행하기 전에, 서버 이동을 위한 하드웨어 정보 설정이 필요합니다. 각 서버의 설정파일은 manifest 디렉터리 하위에 있는 lobby, matchmaker, game 디렉터리 안에 생성됩니다. 먼저, ifconfig명령 혹은 ip link 명령으로 네트워크 인터페이스 이름을 확인해주세요.

```bash
$ ifconfig
# 또는
$ ip link
```

네트워크 인터페이스 이름을 확인하셨으면, lobby 서버의 MANIFEST.json 파일을 열어주세요.

$ sudo vim manifest/lobby/MANIFEST.json
아래의 external_ip_resolvers 내용을 `"nic:{ifconfig에서 확인한 네트워크 인터페이스 이름}"`으로 변경해주세요.

```json
...
"HardwareInfo": {
  "external_ip_resolvers": "aws,nic:eth0"
},
...
```

완료하셨으면, game 서버의 MANIFEST.json 파일도 동일하게 수정해주세요.

```bash
$ sudo vim manifest/game/MANIFEST.json
```

### Monodevelop 에서 실행하기

다음 방법으로 실행할 수 있습니다.

1. Monodevelop 메뉴의 Run -> Start Without Debugging(Ctrl+F5)

기본적으로 Lobby 서버(Flavor) 로 실행되며 다른 서버로 실행하려면 아래와 같이 수정합니다.

1. Monodevelop 메뉴의 Project -> pongcs Options -> Run -> Custom Command 의
2. `${TargetDir}/buildcpp/pongcs.lobby-local -tie_to_parent` 값에서 lobby 를 game 또는 matchmaker 로 변경

### Terminal 에서 실행하기

테스트를 위해서, 각각의 `-local` 스크립트들을 실행시켜주세요.

```bash
$ cd pongcs-build/debug
$ ./pongcs.lobby-local
$ ./pongcs.matchmaker-local
$ ./pongcs.game-local
```

아래와 같이 출력되면 게임 서버가 성공적으로 실행된 것입니다!

```text
...
I0109 00:00:00.094714  9203 manifest_handler.cc:742] Starting SessionService
I0109 00:00:00.094841  9203 session_service.cc:1001] tcp json server start: port(8012)
I0109 00:00:00.094918  9203 session_service.cc:1015] udp json server start: port(8013)
I0109 00:00:00.094981  9203 manifest_handler.cc:742] Starting PongServer
```

## 테스트

먼저, [여기](https://github.com/iFunFactory/game-pong)에서 pong 게임 클라이언트를 다운받아주세요.

다음으로, 다운받은 Pong Client 프로젝트를 실행시켜 Main씬을 로드합니다. `NetworkManager` 오브젝트 의 Server addr값을 현재 서버의 주소로 변경해주세요.

게임을 실행시키고 **[게스트 로그인]** 혹은 **[페이스북 로그인]** 버튼을 누르면 **클라이언트**에서 **lobby 서버**로 로그인 요청을 보내게 됩니다. **lobby 서버**가 요청을 받아 정상적으로 인증되면 클라이언트를 **lobby server**로 이동시키고 클라이언트에서는 **[대전시작]** 버튼이 활성화됩니다. **lobby 서버**에서는 아래와 같은 로그인 성공 메시지를 출력함을 확인할 수 있습니다.

```bash
...
I0412 16:25:51.575713 29140 transport.cc:292] Client plugin version: 186
I0412 16:25:51.577785 29140 session_service.cc:1262] session created: 3645f3c2-820f-42dc-a6e4-1083cbe8aebc
I0412 16:25:51.601130 29206 (Mono)lobby_server.cs:144] Succeed to login: id=2976b445e50b4257b94d51086f437ec25f358671_Desktop
```

**[대전시작]** 버튼을 누르면 **클라이언트**에서 **lobby 서버**로 매치 요청을 보내게 되고, **lobby 서버**는 **matchmaker 서버**에게 매치메이킹을 요청합니다. 매치가 정상적으로 이루어지면 두 **클라이언트**는 **game 서버**로 이동하고 대전이 진행됩니다. 이후 승패가 결정되면 두 클라이언트는 다시 **lobby 서버**로 돌아오게 됩니다.

일정 시간이 지나도 상대를 찾지 못할 경우 Timeout되어 매칭을 취소하게 됩니다.

```bash
...
I0109 00:00:00.632324  9555 (Mono)lobby_server.cs:266] Failed in matchmaking. Timeout: id=4f4ccf9233f6cd83978a5bd21ad41e1e61829d81_Editor
```

**[순위]** 버튼을 누르면 랭킹을 확인할 수 있습니다.

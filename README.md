# AOS/iOS 웹뷰를 위한 Wrapper DLL 생성

### 개요
- 유니티 에셋 스토어에서 구매한 모바일웹뷰 에셋의 부족한 기능을 추가하기 위한 프로젝트입니다
- 웹페이지의 자바스크립트 함수 호출 기능 추가를 목적으로 두고 있습니다
**현재 iOS는 추가 구현 하지 않았습니다**

### 환경설정
- Microsoft Windows 10 Pro
- Microsoft Visual Studio Community 2017 버전 15.9.4
	- VisualStudio.15.Release/15.9.4+28307.222
	- Microsoft .NET Framework 버전 4.7.03056

### 참조 (프로젝트의 외부 참조 세팅)
- 유니티 설치 폴더에서 해당 경로의 dll를 참조 추가해야 합니다.
- UnityEngine.Ui.dll - Editor\Data\UnityExtensions\Unity\GUISystem\Standalone
- UnityEngine.dll - Editor\Data\Managed

### 빌드 방법
- 비주얼 스튜디오의 상단 메뉴 -> 빌드 -> 솔루션 빌드
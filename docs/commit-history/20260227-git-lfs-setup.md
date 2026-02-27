# 2026-02-27 Git LFS 설정

## chore

### Git LFS 설치 및 초기화
- Homebrew로 `git-lfs` 3.7.1 설치
- `git lfs install`로 리포지토리 LFS 초기화

### `.gitattributes` 생성
- 39개 확장자에 대한 LFS 추적 패턴 설정
  - 텍스처/이미지: png, jpg, jpeg, tga, psd, tif, tiff, bmp, exr, hdr
  - 3D 모델: fbx, obj, blend, ma, mb
  - 오디오: wav, mp3, ogg, flac, aiff
  - 비디오: mp4, mov, avi, webm
  - 폰트: ttf, otf
  - Unity 바이너리: unity, prefab, mat, anim, controller, asset, physicMaterial, physicsMaterial2D
  - 네이티브 라이브러리: dll, so, dylib, cubemap, unitypackage
- Unity YAML 머지 전략 설정 (`merge=unityyamlmerge`)

## docs

### TODO 체크리스트 업데이트
- `docs/20260225-project-setup-plan.md`에서 완료 항목 체크 처리
  - `.gitignore` 추가, Git LFS 설정 완료 표시

#!/bin/bash

# 设置颜色输出
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 项目配置
PROJECT_PATH="Image2Display/Image2Display/Image2Display.csproj"
APP_NAME="Image2Display"
BUNDLE_ID="com.chenxublog.image2display"
ICON_PATH="Image2Display/Image2Display/Assets/logo.icns"
CERTIFICATE_NAME="Image2Display-SelfSigned"
SYS_PASSWD=""
BUILD_ARCH="$1"

# 2. 从项目文件中获取版本号
echo -e "${YELLOW}获取项目版本号...${NC}"
VERSION=$(grep -o '<Version>[^<]*' "$PROJECT_PATH" | sed 's/<Version>//')
if [ -z "$VERSION" ]; then
    VERSION=$(grep -o '<AssemblyVersion>[^<]*' "$PROJECT_PATH" | sed 's/<AssemblyVersion>//')
fi
if [ -z "$VERSION" ]; then
    VERSION="1.0.0"
    echo -e "${YELLOW}警告: 无法从项目文件获取版本号，使用默认版本 $VERSION${NC}"
else
    echo -e "${GREEN}获取到版本号: $VERSION${NC}"
fi

# 输出目录
BUILD_DIR="./build"
APP_DIR="$BUILD_DIR/$APP_NAME-$VERSION-$BUILD_ARCH.app"
CONTENTS_DIR="$APP_DIR/Contents"
MACOS_DIR="$CONTENTS_DIR/MacOS"
RESOURCES_DIR="$CONTENTS_DIR/Resources"
DMG_NAME="$APP_NAME-$VERSION-$BUILD_ARCH.dmg"
DMG_TEMP_DIR="$BUILD_DIR/dmg_temp"

echo -e "${BLUE}开始构建 macOS 应用包...${NC}"

# 1. 检查必要文件
echo -e "${YELLOW}检查项目文件...${NC}"
if [ ! -f "$PROJECT_PATH" ]; then
    echo -e "${RED}错误: 找不到项目文件 $PROJECT_PATH${NC}"
    exit 1
fi

if [ ! -f "$ICON_PATH" ]; then
    echo -e "${RED}错误: 找不到图标文件 $ICON_PATH${NC}"
    exit 1
fi

# 3. 清理并创建目录结构
echo -e "${YELLOW}创建目录结构...${NC}"
rm -rf "$BUILD_DIR"
mkdir -p "$MACOS_DIR"
mkdir -p "$RESOURCES_DIR"

# 4. 编译项目
echo -e "${YELLOW}编译项目...${NC}"
dotnet publish "$PROJECT_PATH" \
    -f net9.0 \
    -r "$BUILD_ARCH" \
    --configuration Release \
    --self-contained true \
    -p:PublishTrimmed=true \
    -p:TrimMode=link \
    -o "$BUILD_DIR/publish"

if [ $? -ne 0 ]; then
    echo -e "${RED}编译失败${NC}"
    exit 1
fi

echo -e "${GREEN}编译完成${NC}"

# 5. 复制可执行文件
echo -e "${YELLOW}复制可执行文件...${NC}"
cp -r "$BUILD_DIR/publish/"* "$MACOS_DIR/"

# 确保可执行文件有执行权限
chmod +x "$MACOS_DIR/$APP_NAME"

# 6. 复制图标
echo -e "${YELLOW}复制应用图标...${NC}"
cp "$ICON_PATH" "$RESOURCES_DIR/"

# 7. 创建 Info.plist
echo -e "${YELLOW}创建 Info.plist...${NC}"
cat > "$CONTENTS_DIR/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleIconFile</key>
    <string>logo.icns</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSSupportsAutomaticGraphicsSwitching</key>
    <true/>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.utilities</string>
</dict>
</plist>
EOF

# 8. 创建 PkgInfo
echo -n "APPL????" > "$CONTENTS_DIR/PkgInfo"

# 9. 生成自签名证书（如果不存在）
echo -e "${YELLOW}检查并生成自签名证书...${NC}"
if ! security find-identity -v -p codesigning | grep -q "$CERTIFICATE_NAME"; then
    echo -e "${YELLOW}生成自签名证书...${NC}"
    
    # 创建临时配置文件
    cat > /tmp/cert_config.txt << EOF
[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_req
prompt = no

[req_distinguished_name]
CN = $CERTIFICATE_NAME
O = Chen Xu Blog
C = CN

[v3_req]
keyUsage = keyEncipherment, dataEncipherment, digitalSignature
extendedKeyUsage = codeSigning
EOF

    # 生成私钥和证书
    openssl req -new -x509 -days 365 -nodes \
        -config /tmp/cert_config.txt \
        -keyout /tmp/cert.key \
        -out /tmp/cert.crt

    # 创建 p12 文件
    openssl pkcs12 -export -out /tmp/cert.p12 \
        -inkey /tmp/cert.key \
        -in /tmp/cert.crt \
        -name "$CERTIFICATE_NAME" \
        -passout pass:123456

    security unlock-keychain -p "$SYS_PASSWD" ~/Library/Keychains/login.keychain-db || true

    # 导入到钥匙串
    security import /tmp/cert.p12 -k ~/Library/Keychains/login.keychain-db -P "123456" -T /usr/bin/codesign

    # 设置信任
    security add-trusted-cert -d -r trustRoot -k ~/Library/Keychains/login.keychain-db /tmp/cert.crt

    # 清理临时文件
    rm -f /tmp/cert_config.txt /tmp/cert.key /tmp/cert.crt /tmp/cert.p12

    echo -e "${GREEN}自签名证书已生成并安装${NC}"
else
    echo -e "${GREEN}证书已存在，跳过生成${NC}"
fi

# 10. 代码签名
echo -e "${YELLOW}对应用进行代码签名...${NC}"

# 首先签名所有的可执行文件和库
find "$APP_DIR" -type f \( -name "*.dylib" -o -name "*.so" -o -perm +111 \) -exec codesign --force --sign "$CERTIFICATE_NAME" {} \;

# 然后签名整个应用包
codesign --force --deep --sign "$CERTIFICATE_NAME" "$APP_DIR"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}代码签名完成${NC}"
else
    echo -e "${YELLOW}警告: 代码签名失败，但应用仍可使用${NC}"
fi

# 11. 验证签名
echo -e "${YELLOW}验证应用签名...${NC}"
codesign --verify --verbose "$APP_DIR"
if [ $? -eq 0 ]; then
    echo -e "${GREEN}签名验证通过${NC}"
fi

# 12. 创建 DMG 镜像
echo -e "${YELLOW}创建 DMG 镜像...${NC}"

mkdir -p "$DMG_TEMP_DIR"
cp -r "$APP_DIR" "$DMG_TEMP_DIR/"

# 创建应用程序文件夹的符号链接
ln -s /Applications "$DMG_TEMP_DIR/Applications"

# 创建 DMG
hdiutil create -volname "$APP_NAME" \
    -srcfolder "$DMG_TEMP_DIR" \
    -ov -format UDZO \
    "$BUILD_DIR/$DMG_NAME"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}DMG 镜像创建完成: $BUILD_DIR/$DMG_NAME${NC}"
fi

rm -rf "$DMG_TEMP_DIR"

# 13. 清理编译临时文件
rm -rf "$BUILD_DIR/publish"

echo -e "${GREEN}✅ 构建完成！${NC}"
echo -e "${BLUE}应用位置: $APP_DIR${NC}"
echo -e "${BLUE}版本: $VERSION${NC}"
echo -e "${BLUE}Bundle ID: $BUNDLE_ID${NC}"

# 14. 提供使用说明
echo -e "\n${YELLOW}使用说明:${NC}"
echo -e "1. 应用已打包为: $APP_DIR"
echo -e "2. 可以直接双击运行，或拖拽到 Applications 文件夹"
echo -e "3. 如果系统提示安全警告，请在系统偏好设置 > 安全性与隐私中允许运行"
echo -e "4. 首次运行可能需要在终端中执行: xattr -cr '$APP_DIR'"

echo -e "\n${GREEN}🎉 macOS 应用打包完成！${NC}"
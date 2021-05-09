# This script is needed because OmniSharp will not behave on Connor's Linux box
# and this is the only way he can launch vscode and get any reasonable amount of language support.

DOTNET_BASE=$(dotnet --info | grep "Base Path" | awk '{print $3}')
echo "DOTNET_BASE: ${DOTNET_BASE}"

DOTNET_ROOT=$(echo $DOTNET_BASE | sed -E "s/^(.*)(\/sdk\/[^\/]+\/)$/\1/")
echo "DOTNET_ROOT: ${DOTNET_ROOT}"

export MSBuildSDKsPath=${DOTNET_BASE}Sdks/ 
export DOTNET_ROOT=$DOTNET_ROOT
export PATH=$DOTNET_ROOT:$PATH

code .
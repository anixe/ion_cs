#!/bin/bash
rm -rf ./bin    # cleanup old artifcats
rm -rf ./obj    # cleanup old artifcats

NUGET_SOURCE=https://api.nuget.org/v3/index.json
NUGET_KEY=

CSPROJ=`ls | grep ".csproj$"`

PROJECT_NAME=`echo $CSPROJ | sed "s/.csproj//g"`
PROJECT_VERSION=`cat $CSPROJ | grep "<Version>" | xargs | sed "s/<Version>//g" | sed "s/<\/Version>//g"`

FULL_PACKAGE_NAME="$PROJECT_NAME.$PROJECT_VERSION.nupkg"
FULL_PACKAGE_NAME_SYMBOLS="$PROJECT_NAME.$PROJECT_VERSION.symbols.nupkg"

EXPECTED_PACKAGE_NAME=$"$PROJECT_NAME $PROJECT_VERSION"
EXISTING_PACKAGE_NAME="$(nuget list "$EXPECTED_PACKAGE_NAME" -Source "$NUGET_SOURCE")"

echo ""
echo "The following packages will be published:" 
echo "    - $FULL_PACKAGE_NAME"
echo "    - $FULL_PACKAGE_NAME_SYMBOLS"
echo ""

# Check user's choice.
echo "Do you agree to publish?"
select yn in "yes" "no"; do
    case $yn in
        yes ) echo "" && break;;
        no ) exit;;
    esac
done

echo "Existing package:     $EXISTING_PACKAGE_NAME"
echo "New package:          $EXPECTED_PACKAGE_NAME"
echo

# Check if package does not exist.
if [ "$EXPECTED_PACKAGE_NAME" = "$EXISTING_PACKAGE_NAME" ]
then
    echo "The package already exists. Do you want to replace it?"
    select yn in "yes" "no"; do
        case $yn in
            yes ) echo "" && break;;
            no ) exit;;
        esac
    done
fi

# Prepare packages
dotnet restore || exit 1
dotnet pack -c Release --include-symbols --include-source || exit 1
cd bin/Release || exit 1

# Push packages
dotnet nuget push $FULL_PACKAGE_NAME -k $NUGET_KEY --source $NUGET_SOURCE
dotnet nuget push $FULL_PACKAGE_NAME_SYMBOLS -k $NUGET_KEY --source $NUGET_SOURCE
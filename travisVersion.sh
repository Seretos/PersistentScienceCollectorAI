#!/usr/bin/env sh

red='\033[0;31m'
nc='\033[0m' # No Color

semverParseInto() {
    val="$1";
    if [ "X${val}X" = "XX" ]; then echo "${red}WARN${nc} :: '$val' is not a valid semver"; val="0.0.0"; fi;
    # shellcheck disable=SC2039
    # local RE='[^0-9]*\([0-9]*\)[.]\([0-9]*\)[.]\([0-9]*\)[-]\{0,1\}\([0-9A-Za-z.-]*\)'
    local RE="[^0-9]*\([0-9]\+\)\(\.\([0-9]\+\)\|\)\(\.\([0-9]\+\)\|\)[-]\{0,1\}\([0-9A-Za-z.-]*\)"

    #MAJOR
    eval "$2"="$(echo ${val} | sed -n -e "s#$RE#\1#p")"

    #MINOR
    eval "$3"="$(echo ${val} | sed -n -e "s#$RE#\3#p")"
    eval "$3=\${$3:-0}"

    #PATCH
    eval "$4"="$(echo ${val} | sed -n -e "s#$RE#\5#p")"
    eval "$4=\${$4:-0}"

    #SPECIAL
    eval "$5"="$(echo ${val} | sed -n -e "s#$RE#\6#p")"
}

TRAVIS_MAJOR=0
TRAVIS_MINOR=0
TRAVIS_PATCH=0
TRAVIS_FEATURE=0

semverParseInto "$TRAVIS_BRANCH" TRAVIS_MAJOR TRAVIS_MINOR TRAVIS_PATCH TRAVIS_FEATURE

TRAVIS_RELEASE_VERSION="$TRAVIS_MAJOR.$TRAVIS_MINOR.$TRAVIS_PATCH$TRAVIS_FEATURE"

echo $TRAVIS_RELEASE_VERSION

sed -i -e "s/\"MAJOR\": 0/\"MAJOR\": $TRAVIS_MAJOR/g" PersistentScienceCollectorAI.version
sed -i -e "s/\"MINOR\": 0/\"MINOR\": $TRAVIS_MINOR/g" PersistentScienceCollectorAI.version
sed -i -e "s/\"PATCH\": 0/\"PATCH\": $TRAVIS_PATCH/g" PersistentScienceCollectorAI.version
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

echo <<EOF{
    "NAME": "PersistentScienceCollectorAI", 
    "GITHUB": {
        "REPOSITORY": "PersistentScienceCollectorAI", 
        "USERNAME": "Seretos"
    }, 
    "KSP_VERSION": {
        "MAJOR": 1, 
        "MINOR": 5, 
        "PATCH": 1
    }, 
    "KSP_VERSION_MAX": {
        "MAJOR": 1, 
        "MINOR": 5, 
        "PATCH": 99
    }, 
    "KSP_VERSION_MIN": {
        "MAJOR": 1, 
        "MINOR": 5, 
        "PATCH": 0
    }, 
    "VERSION": {
        "BUILD": 0, 
        "MAJOR": $TRAVIS_MAJOR, 
        "MINOR": $TRAVIS_MINOR, 
        "PATCH": $TRAVIS_PATCH
    }
}
EOF >> PersistentScienceCollectorAI.version
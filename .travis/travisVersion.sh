TRAVIS_MAJOR=0
TRAVIS_MINOR=0
TRAVIS_PATCH=0
TRAVIS_FEATURE=0

function semverParseInto() {
    local RE='[^0-9]*\([0-9]*\)[.]\([0-9]*\)[.]\([0-9]*\)\([0-9A-Za-z-]*\)'
    #MAJOR
    eval $2=`echo $1 | sed -e "s#$RE#\1#"`
    #MINOR
    eval $3=`echo $1 | sed -e "s#$RE#\2#"`
    #MINOR
    eval $4=`echo $1 | sed -e "s#$RE#\3#"`
    #SPECIAL
    eval $5=`echo $1 | sed -e "s#$RE#\4#"`
}

semverParseInto "$TRAVIS_BRANCH" TRAVIS_MAJOR TRAVIS_MINOR TRAVIS_PATCH TRAVIS_FEATURE

TRAVIS_RELEASE_VERSION="$TRAVIS_MAJOR.$TRAVIS_MINOR.$TRAVIS_PATCH$TRAVIS_FEATURE"

echo $TRAVIS_RELEASE_VERSION
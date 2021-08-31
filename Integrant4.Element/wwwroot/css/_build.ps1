Get-ChildItem -Recurse -Filter *.css     | Remove-Item
Get-ChildItem -Recurse -Filter *.css.map | Remove-Item

Write-Output "Building stylesheet: Style.scss"
& sassc -m Style.scss Style.css

Write-Output "Building stylesheet: I4App.scss"
& sassc -m I4App.scss I4App.css

foreach ($stylesheet in $( Get-ChildItem -Recurse 'Overrides/' -Filter *.scss ))
{
    if ( $stylesheet.Name.StartsWith("_"))
    {
        continue
    }

    Write-Output "Building stylesheet: $( $stylesheet.FullName )"
    & sassc -m "$( $stylesheet.FullName )" "$( $stylesheet.DirectoryName )/$( $stylesheet.BaseName ).css"
    if (-not$?)
    {
        exit 2
    }
}

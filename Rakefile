TESTS   = Dir["test/**/*.cs"]
SOURCES = Dir["src/**/*.cs"]
ASSEMBLIES = %w[System.Xml.Linq System.Core HtmlAgilityPack]
LINKED     = %w[lib/HtmlAgilityPack.dll]

def gmcs(*items)
  system *(
    ["gmcs", "-lib:lib"] +
    ASSEMBLIES.map{ |a| "-r:#{a}.dll" } +
    items
  )
  raise "Compilation failed" unless $? == 0
end

file "test.dll" => TESTS + SOURCES do |t|
  gmcs "-t:library", "-pkg:nunit", "-out:test.dll", *(SOURCES + TESTS)
end

task :mono_path do
  ENV["MONO_PATH"] = "lib"
end

task :test => ["test.dll", :mono_path] do
  system "nunit-console", "test.dll"
  rm_rf '%temp%'
end

file "build/iplayer-dl.exe" => SOURCES do |t|
  gmcs "-out:#{t.name}", *t.prerequisites
end

file "release/iplayer-dl.exe" => ["build/iplayer-dl.exe", :mono_path] do |t|
  sh "monomerge.exe -out #{t.name} build/iplayer-dl.exe #{LINKED * " "}"
end

desc "Build executable"
task :default => "build/iplayer-dl.exe"

desc "Link executable with libraries"
task :release => "release/iplayer-dl.exe"

TESTS           = Dir["test/**/*.cs"]
SOURCES         = Dir["src/**/*.cs"]
ASSEMBLIES      = %w[System.Xml.Linq System.Core]
TEST_ASSEMBLIES = %w[]
LINKED          = %w[]
RESOURCES       = Dir["res/*"]

def gmcs(*items)
  sh *(
    ["gmcs", "-lib:lib"] +
    ASSEMBLIES.map{ |a| "-r:#{a}" } +
    RESOURCES.map{ |a| "-resource:#{a}" } +
    items
  )
  raise "Compilation failed" unless $? == 0
end

file "test.dll" => TESTS + RESOURCES + SOURCES do |t|
  test_assemblies = TEST_ASSEMBLIES.map{ |a| "-r:#{a}" }
  gmcs "-t:library", "-pkg:nunit", "-out:test.dll", *(test_assemblies + SOURCES + TESTS)
end

task :mono_path do
  ENV["MONO_PATH"] = "lib"
end

task :test => ["test.dll", :mono_path] do
  system "nunit-console", "test.dll"
  rm_rf '%temp%'
end

file "build/iplayer-dl.exe" => RESOURCES + SOURCES do |t|
  gmcs "-out:#{t.name}", *SOURCES
end

file "release/iplayer-dl.exe" => ["build/iplayer-dl.exe", :mono_path] do |t|
  if LINKED.any?
    sh "monomerge.exe -out #{t.name} build/iplayer-dl.exe #{LINKED * " "}"
  else
    sh "cp build/iplayer-dl.exe #{t.name}"
  end
end

desc "Build executable"
task :default => "build/iplayer-dl.exe"

desc "Link executable with libraries"
task :release => "release/iplayer-dl.exe"

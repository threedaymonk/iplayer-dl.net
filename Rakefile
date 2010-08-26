TESTS           = Dir["test/**/*.cs"]
SOURCES         = Dir["src/**/*.cs"]
ASSEMBLIES      = %w[System.Xml.Linq System.Core]
TEST_ASSEMBLIES = %w[FakeItEasy]
LINKED          = %w[]
RESOURCES       = Dir["res/*"]
DEFINED         = (ENV["DEFINED"] || "").split

file "build" do |t|
  mkdir t.name
end

file "release" do |t|
  mkdir t.name
end

def gmcs(*items)
  sh *(
    ["gmcs", "-lib:lib"] +
    DEFINED.map{ |a| "-d:#{a}" } +
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

file "build/iplayer-dl.exe" => RESOURCES + SOURCES + ["build"] do |t|
  gmcs "-out:#{t.name}", *SOURCES
end

file "release/iplayer-dl.exe" => ["build/iplayer-dl.exe", "release", :mono_path] do |t|
  if LINKED.any?
    sh "monomerge.exe -out #{t.name} build/iplayer-dl.exe #{LINKED * " "}"
  else
    sh "cp build/iplayer-dl.exe #{t.name}"
  end
end

desc "Build and link executable"
task :default => "release/iplayer-dl.exe"

desc "Package"
task :package => "release/iplayer-dl.exe" do |t|
  exe = File.basename(t.prerequisites.first)
  Dir.chdir File.dirname(t.prerequisites.first) do
    version = `mono #{exe} -v`[/(\d+\.?){4}/]
    sh "zip iplayer-dl-#{version}.zip #{exe}"
  end
end

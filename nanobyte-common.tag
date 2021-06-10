<?xml version='1.0' encoding='UTF-8' standalone='yes' ?>
<tagfile doxygen_version="1.9.1" doxygen_gitid="ef9b20ac7f8a8621fcfc299f8bd0b80422390f4b">
  <compound kind="class">
    <name>NanoByte::Common::Threading::ActionExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_action_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static Action&lt; T &gt;</type>
      <name>ToMarshalByRef&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_action_extensions.html</anchorfile>
      <anchor>a3746192fe5477627a1a0e1a13a8eeb4a</anchor>
      <arglist>(this Action&lt; T &gt; action)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::AddToCollection</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_add_to_collection.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::CollectionCommand</base>
    <member kind="function">
      <type></type>
      <name>AddToCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_add_to_collection.html</anchorfile>
      <anchor>aaa95cddc1ba42312535bbdae59ec127b</anchor>
      <arglist>(ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static AddToCollection&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_add_to_collection.html</anchorfile>
      <anchor>a2a7a32d730baeccdf1b134b598d38e4d</anchor>
      <arglist>(ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_add_to_collection.html</anchorfile>
      <anchor>ace825722480f318b8bb4766501d2cb11</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_add_to_collection.html</anchorfile>
      <anchor>a21627915ed8174db463c3d63baa66140</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::AggregateDispatcher</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a1ef54e7a9f0a65d68b4ebac5e7a68533</anchor>
      <arglist>(Action&lt; TSpecific &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a4046fbfbb32b70b4b1d1be2304b9f508</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a9209fbb36aa26a74509a9c1e522d0910</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase, TResult &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>ab7b3eabd6d3019d15de23ae69d6542fd</anchor>
      <arglist>(Func&lt; TSpecific, IEnumerable&lt; TResult &gt;&gt; function)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a31fda94fc816b1c831e3401eac4d9324</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a16cbe636e9733281ffafe53b2d209aea</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>AggregateDispatcher&lt; TModel &gt;</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</filename>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a1ef54e7a9f0a65d68b4ebac5e7a68533</anchor>
      <arglist>(Action&lt; TSpecific &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase, TResult &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>ab7b3eabd6d3019d15de23ae69d6542fd</anchor>
      <arglist>(Func&lt; TSpecific, IEnumerable&lt; TResult &gt;&gt; function)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a4046fbfbb32b70b4b1d1be2304b9f508</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a9209fbb36aa26a74509a9c1e522d0910</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a31fda94fc816b1c831e3401eac4d9324</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a16cbe636e9733281ffafe53b2d209aea</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>AggregateDispatcher&lt; TModel, TView &gt;</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</filename>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a1ef54e7a9f0a65d68b4ebac5e7a68533</anchor>
      <arglist>(Action&lt; TSpecific &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>AggregateDispatcher&lt; TBase, TResult &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>ab7b3eabd6d3019d15de23ae69d6542fd</anchor>
      <arglist>(Func&lt; TSpecific, IEnumerable&lt; TResult &gt;&gt; function)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a4046fbfbb32b70b4b1d1be2304b9f508</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a9209fbb36aa26a74509a9c1e522d0910</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a31fda94fc816b1c831e3401eac4d9324</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_aggregate_dispatcher.html</anchorfile>
      <anchor>a16cbe636e9733281ffafe53b2d209aea</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::AnsiCli</name>
    <filename>class_nano_byte_1_1_common_1_1_ansi_cli.html</filename>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>Prompt&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_ansi_cli.html</anchorfile>
      <anchor>a177f67fbab4509d7b3565befc8aa76cb</anchor>
      <arglist>(TextPrompt&lt; T &gt; prompt, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IRenderable</type>
      <name>Title</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_ansi_cli.html</anchorfile>
      <anchor>adf9d44b3647e3e34eb427be4e4801e37</anchor>
      <arglist>(string title)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IRenderable</type>
      <name>Table&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_ansi_cli.html</anchorfile>
      <anchor>a119bde7691ac58104e897033d58a36e7</anchor>
      <arglist>(IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IRenderable</type>
      <name>Tree&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_ansi_cli.html</anchorfile>
      <anchor>ac7a27d2bfd969032a63ee14e873b9943</anchor>
      <arglist>(NamedCollection&lt; T &gt; data, char separator=Named.TreeSeparator)</arglist>
    </member>
    <member kind="property" static="yes">
      <type>static IAnsiConsole</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_ansi_cli.html</anchorfile>
      <anchor>afd868fdec038ec0fc8c1c2404d4deb9d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::AnsiCliCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_ansi_cli_credential_provider.html</filename>
    <base>NanoByte::Common::Net::CredentialProviderBase</base>
    <member kind="function">
      <type>override NetworkCredential</type>
      <name>GetCredential</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_ansi_cli_credential_provider.html</anchorfile>
      <anchor>ab0636060641dd0941e8c7ed797072ba3</anchor>
      <arglist>(Uri uri, string authType)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::AnsiCliProgress</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress.html</anchorfile>
      <anchor>a1e91cc414a4650cf8fa22ca29e36d78b</anchor>
      <arglist>(TaskSnapshot value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::AnsiCliProgressContext</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress_context.html</filename>
    <member kind="function">
      <type></type>
      <name>AnsiCliProgressContext</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress_context.html</anchorfile>
      <anchor>af03f4743f5f85e4d72ece6d3380c6a64</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress_context.html</anchorfile>
      <anchor>aef526a806ddcbe026ee4ccd99f106a65</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>IProgress&lt; TaskSnapshot &gt;</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress_context.html</anchorfile>
      <anchor>abca11a43ad144da747751330af946460</anchor>
      <arglist>(string description)</arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsFinished</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_progress_context.html</anchorfile>
      <anchor>a99c1bc14718bdd19c2060f1585f7fc40</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::AnsiCliTaskHandler</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</filename>
    <base>NanoByte::Common::Tasks::CliTaskHandler</base>
    <member kind="function">
      <type>override void</type>
      <name>RunTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>ac844d8db8db89268f4ec48e9c3f7fae6</anchor>
      <arglist>(ITask task)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>a101a3ab7d641a1eaafdc051204fb8002</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>a640b5926748d2a893003574845bea004</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>af0785554d0391b9869e8b74c970f9ca7</anchor>
      <arglist>(string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>ade1032baec1d891f928d23703ba07713</anchor>
      <arglist>(string title, NamedCollection&lt; T &gt; data)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>LogHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>a7f5062fdc60e8754dcff2619086799b1</anchor>
      <arglist>(LogSeverity severity, string message)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>a555a3b12590ebbf4d44ebfe04bd134b4</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
    <member kind="property">
      <type>override? ICredentialProvider</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_ansi_cli_task_handler.html</anchorfile>
      <anchor>a32ec215b92ce087977ca7751726ff970</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="struct">
    <name>NanoByte::Common::Info::AppInfo</name>
    <filename>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</filename>
    <member kind="function" static="yes">
      <type>static AppInfo</type>
      <name>Load</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>aba1042b20673a067d760734bb722602a</anchor>
      <arglist>(Assembly? assembly)</arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Name</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a354998d603ae54a0b7fffdb5ac73d871</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>ProductName</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a5279c1ec460835ec8a62b99b2b5da170</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Version</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a3ea5e9874fb540d932aa8f6e4a8a0e8f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>NameVersion</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>aa4632539a9dad650ba3e2f5518e23547</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Copyright</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a55810b1ac66b3901683fc52fa06d4b9e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Description</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a602734cd406019f5bf8ba6dbde41587c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?[]</type>
      <name>Arguments</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>aaa830d09732f5375a8f13fde9229361b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static AppInfo</type>
      <name>Current</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a9b8ae22315dcab7c08c2ccfe91102a3e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static AppInfo</type>
      <name>CurrentLibrary</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_app_info.html</anchorfile>
      <anchor>a5dfac7701dddc4035ba0584a67e41db7</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::AppMutex</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_app_mutex.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Close</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_app_mutex.html</anchorfile>
      <anchor>aaad55dd4a68847930c06205a608ca042</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static AppMutex</type>
      <name>Create</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_app_mutex.html</anchorfile>
      <anchor>a3b9492e0e79abe2d0e47426919bfe8e3</anchor>
      <arglist>([Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>Probe</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_app_mutex.html</anchorfile>
      <anchor>abd216bed38c12309106e66a5f88d1efe</anchor>
      <arglist>([Localizable(false)] string name)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::ArrayExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_array_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static T[]</type>
      <name>Append&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_array_extensions.html</anchorfile>
      <anchor>a5c39cf0e043d1a7e3f6332f494ccf21a</anchor>
      <arglist>(this T[] array, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T[]</type>
      <name>Prepend&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_array_extensions.html</anchorfile>
      <anchor>a10cfb0b8e044703e8f139c5ac47f02e6</anchor>
      <arglist>(this T[] array, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>SequencedEquals&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_array_extensions.html</anchorfile>
      <anchor>adc4c09801fbfe3efb97b15545ac2b903</anchor>
      <arglist>(this T[] first, T[] second, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T[]</type>
      <name>AsArray&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_array_extensions.html</anchorfile>
      <anchor>a61dfb3b949f0cc5691e46c249fcafefc</anchor>
      <arglist>(this ArraySegment&lt; T &gt; segment)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::AsyncFormWrapper</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>AsyncFormWrapper</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a78c03c3cbfa1ee47146947a1c3ab864c</anchor>
      <arglist>(Func&lt; T &gt; init)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Post</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a17b0aa31f2207e8e384c57005081774d</anchor>
      <arglist>(Action&lt; T &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>TResult</type>
      <name>Post&lt; TResult &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a078043bbbd481f7abe1e0025345b8519</anchor>
      <arglist>(Func&lt; T, TResult &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Send</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>ab9504080e284c9d46ecb0d52a5812439</anchor>
      <arglist>(Action&lt; T &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>SendLow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a818e4048fb457272f5852396b613eeef</anchor>
      <arglist>(Action&lt; T &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Close</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a0642f745b94f417f14b897e018cd122b</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_async_form_wrapper.html</anchorfile>
      <anchor>a57675b80a02948db2266f17a0a6ffdd3</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::AtomicRead</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_read.html</filename>
    <member kind="function">
      <type></type>
      <name>AtomicRead</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_read.html</anchorfile>
      <anchor>a9d54b891790f2994c9f5a5bc5a8d24f0</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_read.html</anchorfile>
      <anchor>acbf1ae85a14e12d8804371190126de54</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::AtomicWrite</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</filename>
    <member kind="function">
      <type></type>
      <name>AtomicWrite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>ae2f907e81337093715a264f2fe043982</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Commit</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>adcae13a703f84dfb2a1143c913d0f500</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>a7e6bfc0a4c5461119fb9d25b9ef123eb</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>DestinationPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>a0263bb25d5daa3be815c7ae9e09f36df</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>WritePath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>a3d86d8b1de3efee16e619d35fdeaf816</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsCommitted</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_atomic_write.html</anchorfile>
      <anchor>af7e2133a1ba2d02a6fca19da28e645c9</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::AttributeUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</filename>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; TAttribute &gt;</type>
      <name>GetAttributes&lt; TAttribute, TTarget &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</anchorfile>
      <anchor>a4a85d4fe12338e4a74e3cce7538410ed</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetEnumAttributeValue&lt; TAttribute &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</anchorfile>
      <anchor>ad77f3579694c9b3ba1264b15b3874455</anchor>
      <arglist>(this Enum target, Converter&lt; TAttribute, string &gt; valueRetriever)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static TType</type>
      <name>ConvertFromString&lt; TType &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</anchorfile>
      <anchor>afc17afec63b8fbd4205716b1a2c9efda</anchor>
      <arglist>(this string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ConvertToString&lt; TType &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</anchorfile>
      <anchor>a60351e9db511d15cfba51d4391273c99</anchor>
      <arglist>(this TType value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? TValue</type>
      <name>GetAttributeValue&lt; TAttribute, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_attribute_utils.html</anchorfile>
      <anchor>aa855c7d5bb123b8e98f37acd0e974a9d</anchor>
      <arglist>(this Assembly assembly, [InstantHandle] Func&lt; TAttribute, TValue?&gt; valueRetrieval)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::BinaryStorage</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_binary_storage.html</filename>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadBinary&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_binary_storage.html</anchorfile>
      <anchor>a197016185942bbbd299e182938bb47c6</anchor>
      <arglist>(Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadBinary&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_binary_storage.html</anchorfile>
      <anchor>a99c9c03185827669cd08106a53f82150</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveBinary&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_binary_storage.html</anchorfile>
      <anchor>a50bfdda59a92701f1395d8d4b6228d59</anchor>
      <arglist>(this T data, Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveBinary&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_binary_storage.html</anchorfile>
      <anchor>a2f8f9bf09c138b53c94abc82e3ab1ede</anchor>
      <arglist>(this T data, [Localizable(false)] string path)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::Bucketizer</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type>Bucketizer&lt; T &gt;</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a73c23e9a2c45582527c45c1b2178040c</anchor>
      <arglist>(Predicate&lt; T &gt; predicate, ICollection&lt; T &gt; bucket)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Run</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a01328e1a0282908ddfd9b39fb2cc7657</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>Bucketizer&lt; TElement, TValue &gt;</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a7bf52a048fbe85496c7166f5fb537f6e</anchor>
      <arglist>(TValue value, ICollection&lt; TElement &gt; bucket)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Run</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a01328e1a0282908ddfd9b39fb2cc7657</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Bucketizer&lt; T &gt;</type>
      <name>Bucketize&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>ae939ac77d0e392e2b9117f76638593a2</anchor>
      <arglist>(this IEnumerable&lt; T &gt; elements)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Bucketizer&lt; TElement, TValue &gt;</type>
      <name>Bucketize&lt; TElement, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a9ed0a7b39fac5a2a56759526753b4b8c</anchor>
      <arglist>(this IEnumerable&lt; TElement &gt; elements, Func&lt; TElement, TValue &gt; valueRetriever)</arglist>
    </member>
    <member kind="function" protection="package">
      <type></type>
      <name>Bucketizer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a0b67e9809eaa0a022173685311280672</anchor>
      <arglist>(IEnumerable&lt; T &gt; elements)</arglist>
    </member>
    <member kind="function" protection="package">
      <type></type>
      <name>Bucketizer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucketizer.html</anchorfile>
      <anchor>a62002e43468aed8229d585104c9013f1</anchor>
      <arglist>(IEnumerable&lt; TElement &gt; elements, Func&lt; TElement, TValue &gt; valueRetriever)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::BucketRule</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>BucketRule</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>a781962df821804f6c746852169cf5f18</anchor>
      <arglist>(Predicate&lt; T &gt; predicate, ICollection&lt; T &gt; bucket)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>BucketRule</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>ac67e56e87dd7376d83910b86ecbf0352</anchor>
      <arglist>(TValue value, ICollection&lt; TElement &gt; bucket)</arglist>
    </member>
    <member kind="variable">
      <type>readonly Predicate&lt; T &gt;</type>
      <name>Predicate</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>ae0e6725e47eba6f895a94361ae6e1fc5</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly ICollection&lt; T &gt;</type>
      <name>Bucket</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>ae4b985ff8234d98cc492941a617c852d</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly TValue</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>a35e4561a5162c698a99d123ad01cfacf</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly ICollection&lt; TElement &gt;</type>
      <name>Bucket</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_bucket_rule.html</anchorfile>
      <anchor>a3948788796f964466d320c5767221976</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::CachedCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_cached_credential_provider.html</filename>
    <base>NanoByte::Common::Net::ICredentialProvider</base>
    <member kind="function">
      <type></type>
      <name>CachedCredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_cached_credential_provider.html</anchorfile>
      <anchor>a23bba7a4a08b5ecf6ed4548d90b6be00</anchor>
      <arglist>(ICredentialProvider inner)</arglist>
    </member>
    <member kind="function">
      <type>NetworkCredential?</type>
      <name>GetCredential</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_cached_credential_provider.html</anchorfile>
      <anchor>aa2a10ab8162164f95c53b0017d77e319</anchor>
      <arglist>(Uri uri, string? authType)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>ReportInvalid</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_cached_credential_provider.html</anchorfile>
      <anchor>ad09f87ce3a2e4af78d7060873d4e7857</anchor>
      <arglist>(Uri uri)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::CancellationGuard</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_cancellation_guard.html</filename>
    <member kind="function">
      <type></type>
      <name>CancellationGuard</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_cancellation_guard.html</anchorfile>
      <anchor>a908d5bff9da5d72308ca2e0395fb56d2</anchor>
      <arglist>(CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>CancellationGuard</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_cancellation_guard.html</anchorfile>
      <anchor>a5cad1c29a47499e4ace45532606afff7</anchor>
      <arglist>(CancellationToken cancellationToken, TimeSpan timeout)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_cancellation_guard.html</anchorfile>
      <anchor>a55f061ff308a1865fca8b8bbc862328e</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::ChildProcess</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</filename>
    <member kind="function" virtualness="virtual">
      <type>virtual string</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</anchorfile>
      <anchor>ad94a2047290daf213764a5f233ff6ef0</anchor>
      <arglist>(params string[] arguments)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual ProcessStartInfo</type>
      <name>GetStartInfo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</anchorfile>
      <anchor>aef3d0f1ab000e6d5dacbdb5adb03cb12</anchor>
      <arglist>(params string[] arguments)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>InitStdin</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</anchorfile>
      <anchor>a162ed1c4cf6664f01091ebaf7d4c4f73</anchor>
      <arglist>(StreamWriter writer)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual ? string</type>
      <name>HandleStderr</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</anchorfile>
      <anchor>ac80f7f8948685fa36377010f0aaa2c51</anchor>
      <arglist>(string line)</arglist>
    </member>
    <member kind="property" protection="protected">
      <type>abstract string</type>
      <name>AppBinary</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_child_process.html</anchorfile>
      <anchor>aed11e5ea1800725026588e3b7ebdde2d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::CliProgress</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_progress.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_progress.html</anchorfile>
      <anchor>ad0dd686bbe2cb2c4600a7c0575d4eedb</anchor>
      <arglist>(TaskSnapshot value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::CliTaskHandler</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</filename>
    <base>NanoByte::Common::Tasks::TaskHandlerBase</base>
    <member kind="function">
      <type></type>
      <name>CliTaskHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>a84c4673a859a9999d81b9b83e88a76c9</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>a5dc2558b25a3cd33eb4ff2cbe6c173e5</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>RunTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>a93f25ba50d4e43b62a73d3073b6f3ebd</anchor>
      <arglist>(ITask task)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>a519874c2a55f007242a784166ac3c311</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>ade2f3df7fd3d5e47021b7b063f74dbf9</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>LogHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>ae5d0746dd811f34f98a720bead7ce27b</anchor>
      <arglist>(LogSeverity severity, string message)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>ab20c6d48ff068622b014154986d40d42</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
    <member kind="property">
      <type>override? ICredentialProvider</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>aa48b5b7585885af92e0117fa24dab1c2</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>IsInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_cli_task_handler.html</anchorfile>
      <anchor>a0810ac6a567a31491570b8b756058115</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::CollectionCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_collection_command.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <base>NanoByte::Common::Undo::IValueCommand</base>
    <member kind="function" protection="protected">
      <type></type>
      <name>CollectionCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_collection_command.html</anchorfile>
      <anchor>a912368c40113c22a974b3c451e028e72</anchor>
      <arglist>(ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>readonly ICollection&lt; T &gt;</type>
      <name>Collection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_collection_command.html</anchorfile>
      <anchor>a45954735a1491a55a3c9906c255c89f2</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>readonly T</type>
      <name>Element</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_collection_command.html</anchorfile>
      <anchor>a60a6dace8ebcd82db396e464ea391b98</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>object</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_collection_command.html</anchorfile>
      <anchor>a3a902c4199c350a608c0095e42fccdec</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::CollectionExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_collection_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AddIfNew&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_collection_extensions.html</anchorfile>
      <anchor>a45e042a4399e263f1fbae254c6811bdd</anchor>
      <arglist>(this ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>AddRange&lt; TCollection, TElements &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_collection_extensions.html</anchorfile>
      <anchor>a6c6ed5e5d82751ae63b0c704cabdadab</anchor>
      <arglist>(this ICollection&lt; TCollection &gt; collection, [InstantHandle] IEnumerable&lt; TElements &gt; elements)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>RemoveRange&lt; TCollection, TElements &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_collection_extensions.html</anchorfile>
      <anchor>af4244023373cb4875f918d102228450f</anchor>
      <arglist>(this ICollection&lt; TCollection &gt; collection, [InstantHandle] IEnumerable&lt; TElements &gt; elements)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>RemoveAll&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_collection_extensions.html</anchorfile>
      <anchor>ac8f24ae6d89a3bf8ed378b7d29bc65c9</anchor>
      <arglist>(this ICollection&lt; T &gt; collection, [InstantHandle] Func&lt; T, bool &gt; condition)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::CommandCollector</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_command_collector.html</filename>
    <base>NanoByte::Common::Undo::ICommandExecutor</base>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_collector.html</anchorfile>
      <anchor>a373d80c99c710b0a788459b51615225b</anchor>
      <arglist>(IUndoCommand command)</arglist>
    </member>
    <member kind="function">
      <type>IUndoCommand</type>
      <name>BuildComposite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_collector.html</anchorfile>
      <anchor>a1efc39b48f9c947db0121d95f82ddcb4</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_collector.html</anchorfile>
      <anchor>a905be077c5efc4e6f767aa0a3aac7514</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::CommandManager</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::ICommandManager</base>
    <member kind="function">
      <type></type>
      <name>CommandManager</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a3c470555f23567329b05cef08ea0f5e8</anchor>
      <arglist>(T target, string? path=null)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a3102bb6b9bd8c9f8b22a17da792034b1</anchor>
      <arglist>(IUndoCommand command)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Undo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a1ddeaad2c6969f6b247159b008e136a6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Redo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a4db720c75522d555838a69d2803975c1</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Save</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>acf3b8dbb85c48434946e59449cae6f6c</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static CommandManager&lt; T &gt;</type>
      <name>Load</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>af79800f188e6f0e1400a3ada000850e1</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ICommandManager&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a87ba355f66aa2b7308308e5c370c4ac2</anchor>
      <arglist>(T target, string? path=null)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>ClearUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a06921260a76b1f5716b0f1c364e597ab</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>T?</type>
      <name>Target</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a856a172f170e8891948182cadf284140</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>ae439183138e012452aa45b087a34525d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>UndoEnabled</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a8beb1f1eb038721c7c3040065ad72ff1</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>RedoEnabled</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>ab7a70a4c2c9607f6961d94beb1ce1b4c</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>TargetUpdated</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a8758df0a4be4ea46e443a9cfc869d160</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>UndoEnabledChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>ac88b7a0add36b9ab47f62dfd5c831fed</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>RedoEnabledChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_command_manager.html</anchorfile>
      <anchor>a040b91612d521149b75d822546b57016</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::CompositeCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_composite_command.html</filename>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <member kind="function">
      <type></type>
      <name>CompositeCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_composite_command.html</anchorfile>
      <anchor>a4cf09e87a4c8d4dd8c4b370d21fff64b</anchor>
      <arglist>(params IUndoCommand[] commands)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_composite_command.html</anchorfile>
      <anchor>a0ec10f03c89e2fef0c89c75322b6e6be</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_composite_command.html</anchorfile>
      <anchor>ab6ba4f15b0464e99c4e55f9078430ee2</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::ConcurrentSet</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a85aff2bad87d33cd4f9bc8eef73b5928</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>abd33b2fcaa819f5d063eb8bfb8c47b16</anchor>
      <arglist>(int concurrencyLevel, int capacity)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a5d6bafd985575e5a75a6b22573a9315e</anchor>
      <arglist>(IEqualityComparer&lt; T &gt; comparer)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a73c63d7f6a315a9a083d89bca8f2c2a3</anchor>
      <arglist>(IEnumerable&lt; T &gt; collection)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a40a6cf59838fe3471b0dda7fc2b5a849</anchor>
      <arglist>(IEnumerable&lt; T &gt; collection, IEqualityComparer&lt; T &gt; comparer)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ConcurrentSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a184345bc1e435648230c8730cd95befd</anchor>
      <arglist>(int concurrencyLevel, IEnumerable&lt; T &gt; collection, IEqualityComparer&lt; T &gt; comparer)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerator</type>
      <name>GetEnumerator</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a3d50bc743e5ce57b90c23f99a49e7149</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>aa97ce5167aa60d42e49857a1e0847d23</anchor>
      <arglist>(T item)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Clear</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a5c888657781b111cac49ecbb44fa025a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Contains</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a204a730f16744c1f6790acac468ec3b9</anchor>
      <arglist>(T item)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>CopyTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a1ed642ff5d4e89e5777a364a5d894ed9</anchor>
      <arglist>(T[] array, int arrayIndex)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a783bc177d7bff6dc4e9d79587ab98d41</anchor>
      <arglist>(T item)</arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>Count</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a07e0e78906d3c3f220cc0d1041b59284</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsReadOnly</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_concurrent_set.html</anchorfile>
      <anchor>a89f3d6860dcd3944ec5c6d1e76f9a1a3</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::ConfigurationCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider.html</filename>
    <base>NanoByte::Common::Net::ICredentialProvider</base>
    <member kind="function">
      <type></type>
      <name>ConfigurationCredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider.html</anchorfile>
      <anchor>a99e276726184bc61c80d5f61a87957b5</anchor>
      <arglist>(IConfiguration configuration)</arglist>
    </member>
    <member kind="function">
      <type>NetworkCredential</type>
      <name>GetCredential</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider.html</anchorfile>
      <anchor>ab56e47efb1b3d5ad14a3d0f57a17390d</anchor>
      <arglist>(Uri uri, string authType)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>ReportInvalid</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider.html</anchorfile>
      <anchor>a94b9756807823a55df5cf6f16745544d</anchor>
      <arglist>(Uri uri)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::ConfigurationCredentialProviderRegistration</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider_registration.html</filename>
    <member kind="function" static="yes">
      <type>static IServiceCollection</type>
      <name>ConfigureCredentials</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_configuration_credential_provider_registration.html</anchorfile>
      <anchor>ab565268089a65acb1a9cf30e8004ab7d</anchor>
      <arglist>(this IServiceCollection services, IConfiguration configuration)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::ControlExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_control_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Invoke</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_control_extensions.html</anchorfile>
      <anchor>a13d155a29d88ad483e4d11b5dc781797</anchor>
      <arglist>(this Control control, Action action)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>Invoke&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_control_extensions.html</anchorfile>
      <anchor>a52bbc5206351a214bd665e51ef2fb98b</anchor>
      <arglist>(this Control control, Func&lt; T &gt; action)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static float</type>
      <name>GetDpiScale</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_control_extensions.html</anchorfile>
      <anchor>aecc90486944385fdb9bb7d8135b9e56d</anchor>
      <arglist>(this Control control)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::CopyDirectory</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>CopyDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a93b62063b1aef1243b28cb6ea04b018e</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath, bool preserveDirectoryTimestamps=true, bool overwrite=false)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>ac22fbec2019919a113b987afcb158142</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>CopyFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a3840f3bd3702a7052494057240f3e5f8</anchor>
      <arglist>(FileInfo sourceFile, FileInfo destinationFile)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>CreateSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a2a44da3b3d2e1c810ea817b92a8f1641</anchor>
      <arglist>([Localizable(false)] string linkPath, [Localizable(false)] string linkTarget)</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>ac5fd64609fe2ea31773dfc3e1980f07a</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>af3cebd3fb8fa623d6f007f050248bca4</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>SourcePath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a9023bf2e043784485f485c24c990cbdc</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>DestinationPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>ad4780406f11ff1f12f5cc3fe3a560e0e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>PreserveDirectoryTimestamps</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a31d05749e75a29e6459218cede22bc12</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>Overwrite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_copy_directory.html</anchorfile>
      <anchor>a317996cf6c5efd651bf4f64991e04d7d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::CredentialProviderBase</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_credential_provider_base.html</filename>
    <base>NanoByte::Common::Net::ICredentialProvider</base>
    <member kind="function" virtualness="pure">
      <type>abstract ? NetworkCredential</type>
      <name>GetCredential</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_credential_provider_base.html</anchorfile>
      <anchor>a4c35a613202daa86a1f8e7ed6e15b20a</anchor>
      <arglist>(Uri uri, string authType)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>ReportInvalid</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_credential_provider_base.html</anchorfile>
      <anchor>ad27e4d5641a7cc7c9beeb8ff6e869d30</anchor>
      <arglist>(Uri uri)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>bool</type>
      <name>WasReportedInvalid</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_credential_provider_base.html</anchorfile>
      <anchor>a0cb15819ddfcb1ffc3690b71692d8b67</anchor>
      <arglist>(Uri uri)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::CultureComparer</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_culture_comparer.html</filename>
    <member kind="function">
      <type>int</type>
      <name>Compare</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_culture_comparer.html</anchorfile>
      <anchor>ab84e13739f2dbcd86a15f1dc17ce3388</anchor>
      <arglist>(CultureInfo? x, CultureInfo? y)</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>static readonly CultureComparer</type>
      <name>Instance</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_culture_comparer.html</anchorfile>
      <anchor>ae082095eb1fdefd32fc6760d35a25960</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::CygwinUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_cygwin_utils.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_cygwin_utils.html</anchorfile>
      <anchor>a4401cc1d652916a0a9279a86cf1958ab</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_cygwin_utils.html</anchorfile>
      <anchor>ab527e97ac9402675eb65fee549c5ec06</anchor>
      <arglist>([Localizable(false)] string path, [NotNullWhen(true)] out string? target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_cygwin_utils.html</anchorfile>
      <anchor>a9a05f6138cac1b056f0c71360168dc13</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>static readonly byte[]</type>
      <name>SymlinkCookie</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_cygwin_utils.html</anchorfile>
      <anchor>a2bf1776ec68b3ee93534adf5fbd72184</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::DefaultComparer</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_default_comparer.html</filename>
    <templarg></templarg>
    <member kind="variable" static="yes">
      <type>static readonly DefaultComparer&lt; T &gt;</type>
      <name>Instance</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_default_comparer.html</anchorfile>
      <anchor>ad74c67b2d41f00f2b64a3a1937ec96da</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::DelegatingStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</filename>
    <member kind="function">
      <type>override void</type>
      <name>Flush</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>ae3338d232abb857b24efdf35b49ef6c6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override long</type>
      <name>Seek</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a65f5d57561f116c4ccbf693c6effde96</anchor>
      <arglist>(long offset, SeekOrigin origin)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>SetLength</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a1bb969d37d867796dc2b284f5f2bc1c1</anchor>
      <arglist>(long value)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>ac8ce181bcc398c7fdb2e7b120c48765f</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a64e16bb371e73960b4024f445bba3890</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override Task</type>
      <name>FlushAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a94767df5492b5af52e5b5898d471867d</anchor>
      <arglist>(CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override Task&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a9745cd02b150369e725f8082f1c9634f</anchor>
      <arglist>(byte[] buffer, int offset, int count, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override Task</type>
      <name>WriteAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a1974c64a2821750c2275e1068e2f50c5</anchor>
      <arglist>(byte[] buffer, int offset, int count, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a84f4010231221810ad50dbc005f99399</anchor>
      <arglist>(Span&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>override ValueTask&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a1000c355bbeaa13c893d5cff60b266b9</anchor>
      <arglist>(Memory&lt; byte &gt; buffer, CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a2faf556edcd0b247650a0221ddc742b4</anchor>
      <arglist>(ReadOnlySpan&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>override ValueTask</type>
      <name>WriteAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a9570fcc5ad0ce36eab389c3ba433e700</anchor>
      <arglist>(ReadOnlyMemory&lt; byte &gt; buffer, CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type></type>
      <name>DelegatingStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a297aa092967a9014c7843e621737e209</anchor>
      <arglist>(Stream underlyingStream)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a575ec82353df6c3af581393baed961c6</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>readonly Stream</type>
      <name>UnderlyingStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a69a887f3001ceb8f4fafd1e1d6a4f842</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanRead</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a1308e8ae70f6979d4bc4b01730b53862</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanSeek</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a3a70e75fd90dd9f584f67eb5d4be0ace</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanWrite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a1a05a5bd7b2235358b0d822607ff3e21</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Length</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>ab9d460861cfe02aff0a290469ab8d5da</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>af891e79ad5fffbd7376e00cb4cdc6e77</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override int</type>
      <name>ReadTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>ab68937bafdedf1fa3d578f12ceb1f0b3</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override int</type>
      <name>WriteTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>a02d3aa25c161fe8c8075303bf943a0e9</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Position</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_delegating_stream.html</anchorfile>
      <anchor>adb726d18f64092192cbb3e205b733845</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::DialogTaskHandler</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</filename>
    <base>NanoByte::Common::Tasks::GuiTaskHandlerBase</base>
    <member kind="function">
      <type></type>
      <name>DialogTaskHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>aa72d02ed2d76b448ef71bad7a030e05a</anchor>
      <arglist>(Control owner)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>RunTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>a1bf8916fdda00d5346250466236d3155</anchor>
      <arglist>(ITask task)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>ab4b37ee349cd468726ba518a698015aa</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>aec56366c6635de6b5527f2e3ae7ae32d</anchor>
      <arglist>(string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>a44ac1db9b64dfcb1fe97ded612a7ae5b</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_dialog_task_handler.html</anchorfile>
      <anchor>ae46e49648e9b7e9a7bbaad28aa732d39</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::DictionaryExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>AddRange&lt; TSourceKey, TSourceValue, TTargetKey, TTargetValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a79f8d5e81cbb3b8aa427b1c540874ed0</anchor>
      <arglist>(this IDictionary&lt; TTargetKey, TTargetValue &gt; target, [InstantHandle] IEnumerable&lt; KeyValuePair&lt; TSourceKey, TSourceValue &gt;&gt; source)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static TValue</type>
      <name>GetOrDefault&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a1c47a8fd3471f735b8107aefbcaf66f9</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; dictionary, TKey key)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static TValue</type>
      <name>GetOrAdd&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>acfe483712e30dae802f8bb193cda3fea</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; dictionary, TKey key, [InstantHandle] Func&lt; TValue &gt; valueFactory)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static TValue</type>
      <name>GetOrAdd&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a7f304e66ae874b122a69bcf6e6a5411e</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; dictionary, TKey key)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static async Task&lt; TValue &gt;</type>
      <name>GetOrAddAsync&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a869926d6fb1be29a45bd3ed7c391cb97</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; dictionary, TKey key, Func&lt; Task&lt; TValue &gt;&gt; valueFactory)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static MultiDictionary&lt; TKey, TValue &gt;</type>
      <name>ToMultiDictionary&lt; TSource, TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a99d174851e35e3a7b3ce0b3ed64e38c6</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; TSource &gt; elements, [InstantHandle] Func&lt; TSource, TKey &gt; keySelector, [InstantHandle] Func&lt; TSource, TValue &gt; valueSelector)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>UnsequencedEquals&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>a804822da0d9c3433e3d6231c52fa3fff</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; first, IDictionary&lt; TKey, TValue &gt; second, IEqualityComparer&lt; TValue &gt;? valueComparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>GetUnsequencedHashCode&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>aa46368803f3e53813533a5e485ee70a5</anchor>
      <arglist>([InstantHandle] this IDictionary&lt; TKey, TValue &gt; dictionary, IEqualityComparer&lt; TValue &gt;? valueComparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Deconstruct&lt; TKey, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_dictionary_extensions.html</anchorfile>
      <anchor>aae2e583ffe400fafef627acb99392c23</anchor>
      <arglist>(this KeyValuePair&lt; TKey, TValue &gt; pair, out TKey key, out TValue value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Disposable</name>
    <filename>class_nano_byte_1_1_common_1_1_disposable.html</filename>
    <member kind="function">
      <type></type>
      <name>Disposable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_disposable.html</anchorfile>
      <anchor>a21349f45fda0fb4e46d7f4cc65f19339</anchor>
      <arglist>(Action callback)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_disposable.html</anchorfile>
      <anchor>affb9aa613d167aed1d9998e6dc1495ab</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::DownloadFile</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>DownloadFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>a5d9be0e31d232ecc79a30826c32a4c11</anchor>
      <arglist>(Uri source, Action&lt; Stream &gt; callback, long bytesTotal=-1)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>DownloadFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>af9ca57ba9acaa24776096515f925338b</anchor>
      <arglist>(Uri source, [Localizable(false)] string target, long bytesTotal=-1)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>a6e17d5c5d7a3c4595a79126704bc90dd</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>a197bd2af00a0849682da5fc8dcd2056d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>aab5877b5c790198a9ddaa2ce1c9e3759</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>long</type>
      <name>BytesMaximum</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>a5dc8534bde0fd2b0a64d0d46105be111</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Uri</type>
      <name>Source</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>ab89d10b7b2e55deaf06f10b9239ce6c4</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>NoCache</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>ab523b01e355d8683a02e13a3336a4471</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>WebHeaderCollection?</type>
      <name>ResponseHeaders</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_download_file.html</anchorfile>
      <anchor>ad5f8af03b17e6b186c319f48a894a29a</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::DropDownButton</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_drop_down_button.html</filename>
    <member kind="property">
      <type>bool</type>
      <name>ShowSplit</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_drop_down_button.html</anchorfile>
      <anchor>a951bc8818a69302b23fbdd876dabc420</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::EncodingUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_encoding_utils.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Hash</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>a0d6e72537ab605bdc7fbeff772299e1f</anchor>
      <arglist>(this string value, HashAlgorithm algorithm)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Base64Utf8Encode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>ab2f7fb225d42bf3ba2ea5fc17e1315ab</anchor>
      <arglist>(this string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Base64Utf8Decode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>ad9391fd5992d6f3654fb3d0d5b2a3dd5</anchor>
      <arglist>(this string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Base32Encode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>a815cd86718c0c8059a93c88e314cf037</anchor>
      <arglist>(this byte[] data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Base16Encode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>af36a651927ae1eedc988012a355fa97e</anchor>
      <arglist>(this byte[] data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static byte[]</type>
      <name>Base16Decode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_encoding_utils.html</anchorfile>
      <anchor>aba67443ab5a48b1da9ace01176e605cd</anchor>
      <arglist>(this string encoded)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::EnumDescriptionConverter</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_description_converter.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>override bool</type>
      <name>CanConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_description_converter.html</anchorfile>
      <anchor>a6de4f73f9bb6ce4ee093b53cd06aaa6d</anchor>
      <arglist>(ITypeDescriptorContext context, Type sourceType)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>ConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_description_converter.html</anchorfile>
      <anchor>a4c741ac584b01c6b6e28c9362b6684a0</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>ConvertTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_description_converter.html</anchorfile>
      <anchor>a685312e070ccbc84b28a1421e41c0ad4</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::EnumerableExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ContainsOrEmpty&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a4935cabb794fadd7b7d31b8c768b6b28</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; enumeration, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ContainsAny&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>ae3b09cb255628a1c18a5152bb8ad1da9</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; first, [InstantHandle] IEnumerable&lt; T &gt; second, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>SequencedEquals&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>ab2ddc07de61b4be463177ca20c6262a7</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; first, [InstantHandle] IEnumerable&lt; T &gt; second, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>UnsequencedEquals&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a02be503e329ffee7a938a3e8011fbb6c</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; first, [InstantHandle] IEnumerable&lt; T &gt; second, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>GetSequencedHashCode&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a7537464a17eb3bc14bebd8358a0b4ba3</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; enumeration, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>GetUnsequencedHashCode&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a9f2dedd3396b20fee759f48464c02b92</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; enumeration, IEqualityComparer&lt; T &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>Except&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>ab3325d2cf59caee93ece8d01836980f0</anchor>
      <arglist>(this IEnumerable&lt; T &gt; enumeration, Func&lt; T, bool &gt; predicate)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>Except&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>ab1a9dcd9fa8ecd00a2f89580433f9995</anchor>
      <arglist>(this IEnumerable&lt; T &gt; enumeration, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>Flatten&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>ac4a43fb47504ba67b7280a40fb5cbf69</anchor>
      <arglist>(this IEnumerable&lt; IEnumerable&lt; T &gt;&gt; enumeration)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>WhereNotNull&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>abae29e615f76db81033ed021326c46c2</anchor>
      <arglist>(this IEnumerable&lt; T?&gt; enumeration)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>MaxBy&lt; T, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>abe993714ea3a0451e71806b14f34ccb4</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; enumeration, [InstantHandle] Func&lt; T, TValue &gt; expression, IComparer&lt; TValue &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>MinBy&lt; T, TValue &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>aedda1e70b0af6770e3e329ffc55471a1</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; enumeration, [InstantHandle] Func&lt; T, TValue &gt; expression, IComparer&lt; TValue &gt;? comparer=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>DistinctBy&lt; T, TKey &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a92a962639016b31b73b112177337205a</anchor>
      <arglist>(this IEnumerable&lt; T &gt; enumeration, Func&lt; T, TKey &gt; keySelector)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; TResult &gt;</type>
      <name>TrySelect&lt; TSource, TResult, TException &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>a62963567af0e637885396b88722593e2</anchor>
      <arglist>(this IEnumerable&lt; TSource &gt; source, Func&lt; TSource, TResult &gt; selector, [InstantHandle] Action&lt; TException &gt; exceptionHandler)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>CloneElements&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>aacea63ca7353601bb8933f12f320b8a5</anchor>
      <arglist>(this IEnumerable&lt; T &gt; enumerable)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T &gt;</type>
      <name>TopologicalSort&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>aacfa0d04aa5ee5057caca71595157d65</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; nodes, [InstantHandle] Func&lt; T, IEnumerable&lt; T &gt;&gt; getDependencies)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static async Task</type>
      <name>ForEachAsync&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>aa3af2f522feca783ba1612875c1f056c</anchor>
      <arglist>(this IEnumerable&lt; T &gt; enumerable, Func&lt; T, Task &gt; taskFactory, int maxParallel=0)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; T[]&gt;</type>
      <name>Permutate&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_enumerable_extensions.html</anchorfile>
      <anchor>aff9b16c865fb418448b3d652072a4ea6</anchor>
      <arglist>(this IEnumerable&lt; T &gt; elements)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::EnumExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_enum_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>HasFlag</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_enum_extensions.html</anchorfile>
      <anchor>a2eeaaf0be54f182665d394ece552a77f</anchor>
      <arglist>(this Enum enumRef, Enum flag)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>HasFlag</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_enum_extensions.html</anchorfile>
      <anchor>ae1ffc7406d0cdae1e0f771543ba4a5f0</anchor>
      <arglist>(this ushort enumRef, ushort flag)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>HasFlag</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_enum_extensions.html</anchorfile>
      <anchor>a7b3968f77e6f0bd06b51fb2b832e71a8</anchor>
      <arglist>(this int enumRef, int flag)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::EnumXmlConverter</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_xml_converter.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>override bool</type>
      <name>CanConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_xml_converter.html</anchorfile>
      <anchor>abe225f670676c1f85713cbcd08afe8e5</anchor>
      <arglist>(ITypeDescriptorContext context, Type sourceType)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>ConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_xml_converter.html</anchorfile>
      <anchor>ab275f56ec84f7bb6c588ca289f37b845</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>ConvertTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_enum_xml_converter.html</anchorfile>
      <anchor>a7974e6e862695ea14fe5ff187e08427e</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::ErrorBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_error_box.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Show</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_box.html</anchorfile>
      <anchor>a38db167b2483363ac3cec93231ee8009</anchor>
      <arglist>(IWin32Window? owner, Exception exception, RtfBuilder logRtf)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_box.html</anchorfile>
      <anchor>a4829f4369c1ed2faced2b59ba3b2bc07</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::ErrorReport</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</filename>
    <member kind="property">
      <type>AppInfo</type>
      <name>Application</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</anchorfile>
      <anchor>abfeb83e7b95ba6eeb4a44d9996bfdd04</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>OSInfo</type>
      <name>OS</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</anchorfile>
      <anchor>ab5bba6d54cfd9b5a62efb84056170a15</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>ExceptionInfo?</type>
      <name>Exception</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</anchorfile>
      <anchor>a215499205f4e6a029bbf31d0a3a95bc6</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Log</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</anchorfile>
      <anchor>a53cfa89eaa034777bd23fbbd7eec123e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Comments</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report.html</anchorfile>
      <anchor>ae0302f91f42c37596d5e710123260c76</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::ErrorReportForm</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_error_report_form.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetupMonitoring</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report_form.html</anchorfile>
      <anchor>a31e67cc777a5edd9492c2f84f2d9e711</anchor>
      <arglist>(Uri uploadUri)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report_form.html</anchorfile>
      <anchor>a3817321b0ed32d98678ffcfc947e3c35</anchor>
      <arglist>(Exception ex, Uri uploadUri)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_error_report_form.html</anchorfile>
      <anchor>aaf5d3e8de887f0357196bf6a9ae95b9d</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Info::ExceptionInfo</name>
    <filename>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</filename>
    <member kind="function">
      <type></type>
      <name>ExceptionInfo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>aebed0b3f63d7155b775cfbd292d764e1</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ExceptionInfo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>a1ef585d123a11ead9fb463622e56853f</anchor>
      <arglist>(Exception ex)</arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>ExceptionType</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>aa7d0264416b126b06fa892cda12c8eae</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Message</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>a14e86746798897400686144ec0dd0521</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Source</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>ad3e448524935f8efcbe24424ee8152e9</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>StackTrace</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>ab61c16382366aa86df09186e592dc4b0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>ExceptionInfo?</type>
      <name>InnerException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_info_1_1_exception_info.html</anchorfile>
      <anchor>a4ca0908e527bf89df5b5e7e5c3578322</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::ExceptionUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_exception_utils.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetMessageWithInner</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>af9ad2a3f4e8612557a12bd1685364cdb</anchor>
      <arglist>(this Exception exception)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Exception</type>
      <name>Rethrow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>aff8ae7475eac7cd8004bdda4fabf9445</anchor>
      <arglist>(this Exception exception)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ApplyWithRollback&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>ae15ff09af0e409acac136c2deb0358de</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; elements, [InstantHandle] Action&lt; T &gt; apply, [InstantHandle] Action&lt; T &gt; rollback)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>TryAny&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>a73ac508990b2f5c4f1c643e3d65b85c9</anchor>
      <arglist>([InstantHandle] this IEnumerable&lt; T &gt; elements, [InstantHandle] Action&lt; T &gt; action)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Retry&lt; TException &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>a3090894e17011ed489b47a61e5d9be9d</anchor>
      <arglist>(RetryAction action, int maxRetries=2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static async Task</type>
      <name>ApplyWithRollbackAsync&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>acb70db070a73c617b9be12e8cd2baa65</anchor>
      <arglist>(this IEnumerable&lt; T &gt; elements, Func&lt; T, Task &gt; apply, Func&lt; T, Task &gt; rollback)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static async Task</type>
      <name>TryAnyAsync&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>a7d99561a0593d2b029546b3054fe0f0e</anchor>
      <arglist>(this IEnumerable&lt; T &gt; elements, Func&lt; T, Task &gt; action)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static async Task</type>
      <name>RetryAsync&lt; TException &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_exception_utils.html</anchorfile>
      <anchor>a4869e186f659a9adec39f8ee0771ec79</anchor>
      <arglist>(RetryAsyncAction action, int maxRetries=2)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::ExtraDisposeStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_extra_dispose_stream.html</filename>
    <base>NanoByte::Common::Streams::DelegatingStream</base>
    <member kind="function">
      <type></type>
      <name>ExtraDisposeStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_extra_dispose_stream.html</anchorfile>
      <anchor>ab3ff53d20cd6ef3e208cf7a810a3048e</anchor>
      <arglist>(Stream underlyingStream, Action disposeHandler)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_extra_dispose_stream.html</anchorfile>
      <anchor>a42a3734af6d37e236bddd480e3a4389d</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::FileUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>PathCombine</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a9c2fd428ba076cda733d9c552fffe897</anchor>
      <arglist>(params string[] paths)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>UnifySlashes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a49dddda0a9e4023a70c7431bb0bbed21</anchor>
      <arglist>(string? value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsBreakoutPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a193e02ae6990595a4b7e57a2bc16024e</anchor>
      <arglist>([Localizable(false)] string? path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>RelativeTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a4010ad9567d149ea58e226aadf2d1d9b</anchor>
      <arglist>(this FileSystemInfo target, FileSystemInfo baseRef)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ExistsCaseSensitive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a1d93a7e198924031c6364e0387fba37a</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Touch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a9aef2edf01f639cc886beeda8abd3c72</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static long</type>
      <name>ToUnixTime</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>ab3e41dd32731251c1e165c4cbe4693fa</anchor>
      <arglist>(this DateTime time)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static DateTime</type>
      <name>FromUnixTime</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>aeac8324226b1021ef56aeb41bad45c34</anchor>
      <arglist>(long time)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>DetermineTimeAccuracy</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a44f2e1c239d0f3a851d84d11aaca2e3c</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetTempFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>acae4b872613ae7435b2a0fa8e8e571f1</anchor>
      <arglist>([Localizable(false)] string prefix)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetTempDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a6e8316f6b676acac6c98310cd4f63c76</anchor>
      <arglist>([Localizable(false)] string prefix)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Replace</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a4928c0551b69be75ebc60a90add24675</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>ReadFirstLine</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a2f16a6c9cf4717d6b0ef4b765f30b14a</anchor>
      <arglist>(this FileInfo file, Encoding encoding)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Walk</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a82dba06d1a65fdba5a0059b02ab881ef</anchor>
      <arglist>(this DirectoryInfo directory, [InstantHandle] Action&lt; DirectoryInfo &gt;? dirAction=null, [InstantHandle] Action&lt; FileInfo &gt;? fileAction=null, bool followDirSymlinks=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static DirectoryInfo</type>
      <name>WalkThroughPrefix</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a87fcd54be8eb0503f304e6d0ac3370ee</anchor>
      <arglist>(this DirectoryInfo directory)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string[]</type>
      <name>GetFilesRecursive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>af07b3fcb683e3c4d337828c2606e6358</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ResetAcl</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>ab55452119b82d07f0d8a33d8889b3a1e</anchor>
      <arglist>(this DirectoryInfo directoryInfo)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CanonicalizeAcl</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>ae01bbcf0db2143702ce5fe0db0d6f82d</anchor>
      <arglist>(this ObjectSecurity objectSecurity)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>EnableWriteProtection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>abb12eda2766fe934c5eea1dcfb818469</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>DisableWriteProtection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>acd561976120bd2641fdc2f65957b2a6d</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a8a481444be021b7c7eb5375273078f71</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateHardlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>abd84b7b0c5c1be4992be481a9542901e</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AreHardlinked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>ad9e84d4033e9822b713179f896c10cf6</anchor>
      <arglist>([Localizable(false)] string path1, [Localizable(false)] string path2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsRegularFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a5e870807ef9ea33781c46cd1684ad7e5</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a3abb44f85bc2711551efd22151b0478a</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a0fa1f5adb1df9bee75456d4aac8123b9</anchor>
      <arglist>([Localizable(false)] string path, [NotNullWhen(true)] out string? target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>ac1d8f02d2089723d00aeba1605f56f27</anchor>
      <arglist>(this FileSystemInfo item, [NotNullWhen(true)] out string? target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsExecutable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>aabcb6b56c21cd9cb709535cfa09b526a</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetExecutable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>aa94abce68f26824c04fdda354aed41f6</anchor>
      <arglist>([Localizable(false)] string path, bool executable)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsUnixFS</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>af2cae3f7b9320b0b8ddcf0faade7bc3f</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? byte[]</type>
      <name>ReadExtendedMetadata</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a29616ed77d89e64703330fdbd9d509cd</anchor>
      <arglist>([Localizable(false)] string path, [Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>WriteExtendedMetadata</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_file_utils.html</anchorfile>
      <anchor>a71a160b33c45cf4f9c7bbd936115cd4e</anchor>
      <arglist>([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::FilteredTreeView</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>void</type>
      <name>UpdateList</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>ad70cc8fd5f046e9aec686f7a7e7fa0dd</anchor>
      <arglist>(object? sender=null)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a740e8cf3790cc4b8a49f8cd79b026366</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>ShowSearchBox</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a7678f9e81db86167a5cf376437b3c7a3</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>NamedCollection&lt; T &gt;?</type>
      <name>Nodes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>ad0c9a86db5cd5523e12822929f326e0d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>T?</type>
      <name>SelectedEntry</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a8cfe6f7a762d8acb0efec5cf485f423c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>ICollection&lt; T &gt;</type>
      <name>CheckedEntries</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>ae1dfa6f158099d180a2a3ed890f2e9d8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>char</type>
      <name>Separator</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a727cc5a863aaf5b8e01890b3ac711668</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>CheckBoxes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>adfe9349da20969edad46b9680b601894</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler?</type>
      <name>SelectedEntryChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a6c4d3be3c95afce540849e83f26b7c73</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler?</type>
      <name>SelectionConfirmed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>a77d7183b56e505bb22959b7a97c5465b</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler?</type>
      <name>CheckedEntriesChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_filtered_tree_view.html</anchorfile>
      <anchor>ae65f69d9dcf1b38a6f0a310b11f26b7e</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::FirstExecuteCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</filename>
    <base>NanoByte::Common::Undo::IUndoCommand</base>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</anchorfile>
      <anchor>abb57cea709191e48f6707fe3f94f6d40</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Undo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</anchorfile>
      <anchor>a27595ba4b2d025442a10875caa6da201</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnFirstExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</anchorfile>
      <anchor>ab73f53a3413d037fce5f7068ed7ea8e3</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnRedo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</anchorfile>
      <anchor>a119e9b381e454df6716713b8548dfb08</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_first_execute_command.html</anchorfile>
      <anchor>ae50e9f5f6348c077cdd73a63ddbb2e6c</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::ForEachTask</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>ForEachTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</anchorfile>
      <anchor>a87689d55b0e3efb70db74e302fb2d594</anchor>
      <arglist>([Localizable(true)] string name, IEnumerable&lt; T &gt; target, Action&lt; T &gt; work)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ForEachTask&lt; T &gt;</type>
      <name>Create&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</anchorfile>
      <anchor>a547769d3838dcaa00b30e2ccd42fcea1</anchor>
      <arglist>([Localizable(true)] string name, IEnumerable&lt; T &gt; target, Action&lt; T &gt; work)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</anchorfile>
      <anchor>a01c8e70bbadf01c94b7d900dff11f47b</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</anchorfile>
      <anchor>af34fc62aab78197e7d4f615f2e39d8cc</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_for_each_task.html</anchorfile>
      <anchor>ae02634d2907b6c97c3df6d4edcf5f6d4</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::FuncExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_func_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static Func&lt; TIn, TOut &gt;</type>
      <name>ToMarshalByRef&lt; TIn, TOut &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_func_extensions.html</anchorfile>
      <anchor>ab085adeb855958e9e8c44d66fd58e301</anchor>
      <arglist>(this Func&lt; TIn, TOut &gt; func)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::GuiTaskHandlerBase</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</filename>
    <base>NanoByte::Common::Tasks::TaskHandlerBase</base>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a7d7cb9848bfc58ce9d8ddbc24df767d1</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a475bead06648824f5458544d16f18e7a</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>af65cac5a7b89ee51b844f7bd7bc595d4</anchor>
      <arglist>(string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>ac2da2e9bf75c9fc5b74bec4fb9becd1e</anchor>
      <arglist>(string title, NamedCollection&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>af89e49aab0dd83f20b0cf711400bbd6e</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type></type>
      <name>GuiTaskHandlerBase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>af16e018ceca07629eeadc3c57f38ea6d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>LogHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a6898195e207db13ba37b92a0eacc5a67</anchor>
      <arglist>(LogSeverity severity, string message)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a1b1d7b494a1959eb90b19e62e8fad709</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>readonly RtfBuilder</type>
      <name>LogRtf</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a80fcf5897af72240d03c3d8eea1eae4d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override? ICredentialProvider</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>a9d540833c2814c7b8d00fbf991bf3506</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>IsInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_gui_task_handler_base.html</anchorfile>
      <anchor>ab72d79afe67920561dda93c798cbe8e5</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::HintTextBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</filename>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnEnter</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>aafaf42691c34d8013b1c9b934470a10d</anchor>
      <arglist>(EventArgs e)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnLeave</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a57a7eb51c3cdf0c0267671f8c58e6f1d</anchor>
      <arglist>(EventArgs e)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnTextChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a5ee4975a6be7d56e9e53d7a4d08c246f</anchor>
      <arglist>(EventArgs e)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnBackColorChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a0218e66f103818338052149dc82ff8df</anchor>
      <arglist>(EventArgs e)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>OnClearButtonClicked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a657e81ec7d3c8b2339ec17fb13d38e22</anchor>
      <arglist>(EventArgs e)</arglist>
    </member>
    <member kind="property">
      <type>new Color</type>
      <name>ForeColor</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a062e8db6ff019512d42bf31f1ea182d6</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override string?</type>
      <name>Text</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a41b8ed22a4dfb91805969fe4eac95797</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>HintText</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>aacd69d28e736ee2e0bf61910c5bb67f1</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsHintTextVisible</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a40e11d9c6fd64ad5337b871d7df52e2d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>ShowClearButton</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>a9e9e7c796699c7a94a653d36194fae23</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler?</type>
      <name>ClearButtonClicked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_hint_text_box.html</anchorfile>
      <anchor>ae5ac872f64698514d173c29d37ec9284</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Dispatch::IChangeNotify</name>
    <filename>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_change_notify.html</filename>
    <templarg>TSender</templarg>
    <member kind="event">
      <type>Action&lt; TSender &gt;</type>
      <name>Changed</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_change_notify.html</anchorfile>
      <anchor>a4619c65d3f30f0f4ebcd70af29aaf9f0</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; TSender &gt;</type>
      <name>ChangedRebuild</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_change_notify.html</anchorfile>
      <anchor>a5b3358caed08ac70d714da2f87dc9471</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::ICloneable</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_cloneable.html</filename>
    <templarg>T</templarg>
    <member kind="function">
      <type>T</type>
      <name>Clone</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_cloneable.html</anchorfile>
      <anchor>abd38d89c75cbd127afe6a49a80d3c1cd</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>ICloneable&lt; LocalizableString &gt;</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_cloneable.html</filename>
    <member kind="function">
      <type>T</type>
      <name>Clone</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_cloneable.html</anchorfile>
      <anchor>abd38d89c75cbd127afe6a49a80d3c1cd</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>ICloneable&lt; LocalizableStringCollection &gt;</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_cloneable.html</filename>
    <member kind="function">
      <type>T</type>
      <name>Clone</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_cloneable.html</anchorfile>
      <anchor>abd38d89c75cbd127afe6a49a80d3c1cd</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>ICloneable&lt; XmlDictionary &gt;</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_cloneable.html</filename>
    <member kind="function">
      <type>T</type>
      <name>Clone</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_cloneable.html</anchorfile>
      <anchor>abd38d89c75cbd127afe6a49a80d3c1cd</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>ICloneable&lt; XmlDictionaryEntry &gt;</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_cloneable.html</filename>
    <member kind="function">
      <type>T</type>
      <name>Clone</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_cloneable.html</anchorfile>
      <anchor>abd38d89c75cbd127afe6a49a80d3c1cd</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Undo::ICommandExecutor</name>
    <filename>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_executor.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_executor.html</anchorfile>
      <anchor>a266a7612c877581a0a53658e4a064fdc</anchor>
      <arglist>(IUndoCommand command)</arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Path</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_executor.html</anchorfile>
      <anchor>a10b11e4920473a5ee3e905acbc0510e5</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Undo::ICommandManager</name>
    <filename>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::ICommandExecutor</base>
    <member kind="function">
      <type>void</type>
      <name>Undo</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>a3c314038cbc0bc072af2b1d043a2db8c</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Redo</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>aaa88cd54eae9976f680a669c732c503a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Save</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>a34f62c0114c72ebaa4bedccc4b068fb1</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="property">
      <type>T?</type>
      <name>Target</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>ae2219c8392ea2d59b55d0a8bea9a5801</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>UndoEnabled</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>a67ecdac699dc44b1623f93d48afdcd5e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>RedoEnabled</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>adb180fe6aa9466c7502c8aa1bcc71970</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>TargetUpdated</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>a383521a881a73b6fcd119e1ee3e26ad6</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>UndoEnabledChanged</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>acea87122e95b38a7d1669f28bcd5f1ca</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>RedoEnabledChanged</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_command_manager.html</anchorfile>
      <anchor>af1af68d0291f9094a63258de70e4f93c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Controls::IContextMenu</name>
    <filename>interface_nano_byte_1_1_common_1_1_controls_1_1_i_context_menu.html</filename>
    <member kind="function">
      <type>ContextMenuStrip?</type>
      <name>GetContextMenu</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_controls_1_1_i_context_menu.html</anchorfile>
      <anchor>aeddcefff3e24d6b6bee3826e4cd54e09</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Net::ICredentialProvider</name>
    <filename>interface_nano_byte_1_1_common_1_1_net_1_1_i_credential_provider.html</filename>
    <member kind="function">
      <type>void</type>
      <name>ReportInvalid</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_net_1_1_i_credential_provider.html</anchorfile>
      <anchor>a70093207e464811f8ef44b74dc30d9ac</anchor>
      <arglist>(Uri uri)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Controls::IEditorDialog</name>
    <filename>interface_nano_byte_1_1_common_1_1_controls_1_1_i_editor_dialog.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>DialogResult</type>
      <name>ShowDialog</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_controls_1_1_i_editor_dialog.html</anchorfile>
      <anchor>ad87c4fb1e5b1dbac85288808f9c64f5e</anchor>
      <arglist>(IWin32Window owner, T element)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::IHighlightColor</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_highlight_color.html</filename>
    <member kind="property">
      <type>Color</type>
      <name>HighlightColor</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_highlight_color.html</anchorfile>
      <anchor>adf0e0903eaba6bad726b9144e56d115e</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Drawing::ImageExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_drawing_1_1_image_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static Image</type>
      <name>Scale</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_drawing_1_1_image_extensions.html</anchorfile>
      <anchor>a1ff78b66ddbcec680770392e2a32895c</anchor>
      <arglist>(this Image image, float factor)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Dispatch::IMergeable</name>
    <filename>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_mergeable.html</filename>
    <templarg></templarg>
    <member kind="property">
      <type>string</type>
      <name>MergeID</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_mergeable.html</anchorfile>
      <anchor>ab0785a3e1b71cdf0228f3a48326f95f0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>DateTime</type>
      <name>Timestamp</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_dispatch_1_1_i_mergeable.html</anchorfile>
      <anchor>ad505f90ef74a531e8ef8d1dec28c45e2</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::INamed</name>
    <filename>interface_nano_byte_1_1_common_1_1_i_named.html</filename>
    <member kind="property">
      <type>string</type>
      <name>Name</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_i_named.html</anchorfile>
      <anchor>aeb9037a237a3ec5f1b6585d3de1f1478</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::InputBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_input_box.html</filename>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>Show</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_input_box.html</anchorfile>
      <anchor>a2ba257b2a2ab1961d5f6bcb50d8628f9</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string title, [Localizable(true)] string prompt, [Localizable(true)] string? defaultText=null, bool password=false)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_input_box.html</anchorfile>
      <anchor>a4685654fa067ac0d071f833a79fe015e</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Tasks::ITask</name>
    <filename>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Run</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task.html</anchorfile>
      <anchor>a48ba2c2c7cb446fd56bcdd30c5bae9c7</anchor>
      <arglist>(CancellationToken cancellationToken=default, ICredentialProvider? credentialProvider=null, IProgress&lt; TaskSnapshot &gt;? progress=null)</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>Name</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task.html</anchorfile>
      <anchor>ace54bbfa08e3ca2033f198d8fc2d22fe</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>object?</type>
      <name>Tag</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task.html</anchorfile>
      <anchor>ab76de4aec6d9072547cc4ba09caee448</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>CanCancel</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task.html</anchorfile>
      <anchor>a84cdf3c7ce608f52a80302ade2cff0ff</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Tasks::ITaskHandler</name>
    <filename>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</filename>
    <member kind="function">
      <type>void</type>
      <name>RunTask</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a030bf477768fb11e4190ff8cfc0bb101</anchor>
      <arglist>(ITask task)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Ask</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>ad3aec6e14a708fbb3a0f8b6e48d976ec</anchor>
      <arglist>([Localizable(true)] string question, bool? defaultAnswer=null, [Localizable(true)] string? alternateMessage=null)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Output</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a45a8fcba8a6f3264759bace3af84dcae</anchor>
      <arglist>([Localizable(true)] string title, [Localizable(true)] string message)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a5870955982dfad270c72fc37ad036ea1</anchor>
      <arglist>([Localizable(true)] string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a5dea9b9506cfcc71e56a5160f6d91860</anchor>
      <arglist>([Localizable(true)] string title, NamedCollection&lt; T &gt; data)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Error</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>add8e8c8a7147e0248435a78704dd5eec</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="property">
      <type>CancellationToken</type>
      <name>CancellationToken</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a78459a15843363a15b2db02889f7cb8c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>ICredentialProvider?</type>
      <name>CredentialProvider</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>aec7dcfd28588e8cd31335981efc9d317</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Verbosity</type>
      <name>Verbosity</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_tasks_1_1_i_task_handler.html</anchorfile>
      <anchor>a8af593e6bdef905390eeb42171c2bcdc</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Controls::ITouchControl</name>
    <filename>interface_nano_byte_1_1_common_1_1_controls_1_1_i_touch_control.html</filename>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;</type>
      <name>TouchDown</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_controls_1_1_i_touch_control.html</anchorfile>
      <anchor>a7384225afc52a5ed230b9dfa627259c1</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;</type>
      <name>TouchUp</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_controls_1_1_i_touch_control.html</anchorfile>
      <anchor>a5548842ef4d44d14651c37352528f687</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;</type>
      <name>TouchMove</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_controls_1_1_i_touch_control.html</anchorfile>
      <anchor>ad45fdb90c692264647e047101e59f5a8</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Undo::IUndoCommand</name>
    <filename>interface_nano_byte_1_1_common_1_1_undo_1_1_i_undo_command.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_undo_command.html</anchorfile>
      <anchor>abb70b9ce1b57dde581dc31b00dbae9a1</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Undo</name>
      <anchorfile>interface_nano_byte_1_1_common_1_1_undo_1_1_i_undo_command.html</anchorfile>
      <anchor>a3e25de0bac0fcd50016bf656e48bc473</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::Common::Undo::IValueCommand</name>
    <filename>interface_nano_byte_1_1_common_1_1_undo_1_1_i_value_command.html</filename>
    <base>NanoByte::Common::Undo::IUndoCommand</base>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::JsonStorage</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</filename>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a669a20b286ced2856358243c1cfe578a</anchor>
      <arglist>(Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a7d67d56c249348ef22a5383f43b67d5b</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>FromJsonString&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a7fda8b50140e1ed05adf4c1c1177bc94</anchor>
      <arglist>([Localizable(false)] string data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>FromJsonString&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>aa3c03b098e6e74c1ae8814adef393426</anchor>
      <arglist>(string data, T anonymousType)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>aa20fb69f206dfc19fc884596e8b59a74</anchor>
      <arglist>(this T data, Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a1f3a536d9ca34beb24e6834bb3ff86c2</anchor>
      <arglist>(this T data, [Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ToJsonString&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a9468887e0350fa116c67657effdae6aa</anchor>
      <arglist>(this T data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>ReparseAsJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>aeedf232efad8774495e75945db057153</anchor>
      <arglist>(this object data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>ReparseAsJson&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_json_storage.html</anchorfile>
      <anchor>a154f0a6cc4f50dc60a64ea99fc468185</anchor>
      <arglist>(this object data, T anonymousType)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::KeyEqualityComparer</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_key_equality_comparer.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>KeyEqualityComparer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_key_equality_comparer.html</anchorfile>
      <anchor>aa6c48ec2258e6495e615bee1e744ddb8</anchor>
      <arglist>(Func&lt; T, TKey &gt; keySelector)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Languages</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_languages.html</filename>
    <member kind="function" static="yes">
      <type>static CultureInfo</type>
      <name>FromString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_languages.html</anchorfile>
      <anchor>a036978f4b3b10da33957b7e7de0da239</anchor>
      <arglist>(string langCode)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetUI</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_languages.html</anchorfile>
      <anchor>a9f23a516a79f03eeebe3488b4c98d588</anchor>
      <arglist>(CultureInfo culture)</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>static readonly IEnumerable&lt; CultureInfo &gt;</type>
      <name>AllKnown</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_languages.html</anchorfile>
      <anchor>a8a35791f636b74b6723ac3aa7609073c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::LanguageSet</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</filename>
    <member kind="function">
      <type></type>
      <name>LanguageSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a6e18e2d1d2f951eaf556f3716e8d5af0</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>LanguageSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a147707ba3527b89f212c05cf8003ecb3</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; CultureInfo &gt; collection)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>LanguageSet</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a930a888c88faa79a1b244b83bd816b7c</anchor>
      <arglist>(string value)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a5c65b79d10b1637afd75008e6ebf69cb</anchor>
      <arglist>(string langCode)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>ContainsAny</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a7e2aaa501e143422b2388211737ddd5f</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; CultureInfo &gt; targets, bool ignoreCountry=false)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_language_set.html</anchorfile>
      <anchor>a06253a0684b00a5271a86465c8cbb484</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::LanguageSetEditor</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_language_set_editor.html</filename>
    <member kind="function">
      <type>override UITypeEditorEditStyle</type>
      <name>GetEditStyle</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_language_set_editor.html</anchorfile>
      <anchor>ae10ae534ea9371ae012032eda75ca78b</anchor>
      <arglist>(ITypeDescriptorContext context)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>EditValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_language_set_editor.html</anchorfile>
      <anchor>a22b28510c9f7bbaed774ffd35a2b5fbb</anchor>
      <arglist>(ITypeDescriptorContext context, IServiceProvider provider, object value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::ListExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>AddRange&lt; TList, TElements &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a211b2323d2bc1121cfdda1f3e11a1eab</anchor>
      <arglist>(this IList&lt; TList &gt; list, [InstantHandle] IEnumerable&lt; TElements &gt; elements)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>RemoveLast&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a96ca1eeb483bdee8f732aa65dab811d1</anchor>
      <arglist>(this List&lt; T &gt; list, int number=1)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AddOrReplace&lt; T, TKey &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a8166ef5c1c32e4635b48fcc60b9bb1fc</anchor>
      <arglist>(this List&lt; T &gt; list, T element, [InstantHandle] Func&lt; T, TKey &gt; keySelector)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AddOrReplace&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a92e0271f83a366c8f45409b963d13253</anchor>
      <arglist>(this List&lt; T &gt; list, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IList&lt; T &gt;</type>
      <name>GetAddedElements&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a4e8fb5e1243bf583a24877064db8b111</anchor>
      <arglist>(this IList&lt; T &gt;? newList, IList&lt; T &gt;? oldList, IComparer&lt; T &gt; comparer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IList&lt; T &gt;</type>
      <name>GetAddedElements&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_list_extensions.html</anchorfile>
      <anchor>a24f7e1eaddfb7f07ea1787b05092d436</anchor>
      <arglist>(this IList&lt; T &gt;? newList, IList&lt; T &gt;? oldList)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::LocalizableString</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</filename>
    <base>ICloneable&lt; LocalizableString &gt;</base>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a13ee89f1db7210a27060faa1d99b7126</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Equals</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a6128bd6045c6afff01c73e4458a2a7f7</anchor>
      <arglist>(LocalizableString? other)</arglist>
    </member>
    <member kind="function">
      <type>override bool</type>
      <name>Equals</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a8c00755ef45603d7d944dc9d456c2334</anchor>
      <arglist>(object? obj)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>GetHashCode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a7c2e893f51a4db89eb5146a8ae1e69da</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>LocalizableString</type>
      <name>Clone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a3dcd7d4c4bccfcc3dcc95a20241fa176</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>static readonly CultureInfo</type>
      <name>DefaultLanguage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a9fd69184ba530683e73e6949e4c9dd35</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a4fb5187fed2f75f449069766dcd6d628</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>CultureInfo?</type>
      <name>Language</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a45f2720dfa527a7d2d67627161f875f6</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string??</type>
      <name>LanguageString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string.html</anchorfile>
      <anchor>a6d3102c421d234039bc932a992ffca20</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::LocalizableStringCollection</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</filename>
    <base>ICloneable&lt; LocalizableStringCollection &gt;</base>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a5cb3367db832aab72d81057014358a42</anchor>
      <arglist>([Localizable(false)] string language, string? value)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a9a8460ee586fb29c526d8dbb5df5109b</anchor>
      <arglist>(string value)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>ContainsExactLanguage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a52e094131edd210b778d0c4e82f6c934</anchor>
      <arglist>(CultureInfo language)</arglist>
    </member>
    <member kind="function">
      <type>string?</type>
      <name>GetExactLanguage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a715c40db10e06f66a680259a68683aa3</anchor>
      <arglist>(CultureInfo language)</arglist>
    </member>
    <member kind="function">
      <type>string?</type>
      <name>GetBestLanguage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>aa5f59acbfb89bd28ef6f1c873ea3e28f</anchor>
      <arglist>(CultureInfo language)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Set</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a6ad0e2cefd3f7b26abdb5710b8e5c565</anchor>
      <arglist>(CultureInfo language, string? value)</arglist>
    </member>
    <member kind="function">
      <type>LocalizableStringCollection</type>
      <name>Clone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_localizable_string_collection.html</anchorfile>
      <anchor>a01af9f6ce1af6aa55928087dc6903082</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::Locations</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>OverrideInstallBase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a487862997982d68dd73eb6d713912e44</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetCacheDirPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>ae0519e3fd1e0e11ce4eea299ee2cc453</anchor>
      <arglist>([Localizable(false)] string appName, bool machineWide, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetSaveConfigPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a9e2104a3ed8e7e2ca5785d348fc900a9</anchor>
      <arglist>([Localizable(false)] string appName, bool isFile, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetSaveSystemConfigPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a88ce8d5d90c3f5cb68171f7745681706</anchor>
      <arglist>([Localizable(false)] string appName, bool isFile, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; string &gt;</type>
      <name>GetLoadConfigPaths</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a6e7acd0987dc2ee6a6646251f60b9bde</anchor>
      <arglist>([Localizable(false)] string appName, bool isFile, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetSaveDataPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a28a7452ac7fbea632c13577c467a35e3</anchor>
      <arglist>([Localizable(false)] string appName, bool isFile, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IEnumerable&lt; string &gt;</type>
      <name>GetLoadDataPaths</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>aee008f835e8aa49da2ad1346f9a31357</anchor>
      <arglist>([Localizable(false)] string appName, bool isFile, params string[] resource)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetInstalledFilePath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>ab10477e3427953e64e9ea0b99fecd9c1</anchor>
      <arglist>([Localizable(false)] string fileName)</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>PortableFlagName</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a8fe4225f286bce5978129a7935134a0b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>InstallBase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a199dfc5c64a948d20876255ca95f7235</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsPortable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>afefce137b156f55469338cb61f21dd4f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>PortableBase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a12d1ba9aa848f3245720fd0cac4a71cd</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>HomeDir</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a6aa8b4886c23b38b00be195e5392cf5d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>UserConfigDir</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a22943d598fb2d5c8f4cc32e12059a807</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>UserDataDir</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>adaf59fbe3c52d02ca48143ead03c615d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>UserCacheDir</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a8d1d78b405e13bd9b59ace35e391f00c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>SystemConfigDirs</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a23009c2b32fc236ce2c682ecbe1293eb</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>SystemDataDirs</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a876ae993e8797488200adba0a1142a4d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>SystemCacheDir</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations.html</anchorfile>
      <anchor>a27ac2f0e0529b8e7168a0f7af5c08460</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::LocationsRedirect</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_locations_redirect.html</filename>
    <base>NanoByte::Common::Storage::TemporaryDirectory</base>
    <member kind="function">
      <type></type>
      <name>LocationsRedirect</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations_redirect.html</anchorfile>
      <anchor>ac45d7c0009e2b357adc08913cb71666a</anchor>
      <arglist>(string prefix)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_locations_redirect.html</anchorfile>
      <anchor>ab2b1643c6f4d6f913999d360e3ff7244</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Log</name>
    <filename>class_nano_byte_1_1_common_1_1_log.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Debug</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a30a7c181ba4e272296e9bb91939ead81</anchor>
      <arglist>(string message)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Debug</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a43378bcc15a89a7694aa401c464e9fb7</anchor>
      <arglist>(Exception ex)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Info</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a72069699fae53bdfdc4484e98eb37d0f</anchor>
      <arglist>(string message)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Info</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a0da9a18192b046c62625dececd1aa0c1</anchor>
      <arglist>(Exception ex)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Warn</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a4d9b13518bbda813500e636af2129cc3</anchor>
      <arglist>(string message)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Warn</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a003d6aa428d40bdb486050830646f0ab</anchor>
      <arglist>(Exception ex)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>ae2f76bb6e2e8b09cd403c5def0775acf</anchor>
      <arglist>(string message)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>aa53e2863c4d3d5929de4ff20b8a33b38</anchor>
      <arglist>(Exception ex)</arglist>
    </member>
    <member kind="property" static="yes">
      <type>static ? LogEntryEventHandler</type>
      <name>Handler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>ae1adf1f6b640e3d0e087109ccbe54614</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>Content</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_log.html</anchorfile>
      <anchor>a50a86948f98dc144c1e6bad22b27eb11</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::MarshalNoTimeout</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_marshal_no_timeout.html</filename>
    <member kind="function">
      <type>override? object</type>
      <name>InitializeLifetimeService</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_marshal_no_timeout.html</anchorfile>
      <anchor>aeb12b2b37b3b4d30c4d83d65c8e47ba0</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::MathUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_math_utils.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EqualsTolerance</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_math_utils.html</anchorfile>
      <anchor>ae716ad615105e250b13df8a924ab8608</anchor>
      <arglist>(this float a, float b, float tolerance=0.00001f)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EqualsTolerance</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_math_utils.html</anchorfile>
      <anchor>ab0dff10e5b9c07e38bfdbd0fa3e486fa</anchor>
      <arglist>(this double a, double b, double tolerance=0.00001)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Size</type>
      <name>Multiply</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_math_utils.html</anchorfile>
      <anchor>ac9a2dd734ebf498533614929b130e7a4</anchor>
      <arglist>(this Size size, float factor)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::Merge</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_merge.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>TwoWay&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_merge.html</anchorfile>
      <anchor>af833eb79391f973331bc732dbd0a4d94</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; theirs, [InstantHandle] IEnumerable&lt; T &gt; mine, [InstantHandle] Action&lt; T &gt; added, [InstantHandle] Action&lt; T &gt; removed)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>TwoWay&lt; T, TAdded, TRemoved &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_merge.html</anchorfile>
      <anchor>a55fb6353bc94e6e179df3b2e59671032</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; theirs, [InstantHandle] IEnumerable&lt; T &gt; mine, ICollection&lt; TAdded &gt; added, ICollection&lt; TRemoved &gt; removed)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ThreeWay&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_merge.html</anchorfile>
      <anchor>a38cfd3e64cd33771c1129828861a9422</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; reference, [InstantHandle] IEnumerable&lt; T &gt; theirs, [InstantHandle] IEnumerable&lt; T &gt; mine, [InstantHandle] Action&lt; T &gt; added, [InstantHandle] Action&lt; T &gt; removed)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ThreeWay&lt; T, TAdded, TRemoved &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_merge.html</anchorfile>
      <anchor>aecfe23fbb17c15c9caa2faba843a9af0</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; reference, [InstantHandle] IEnumerable&lt; T &gt; theirs, [InstantHandle] IEnumerable&lt; T &gt; mine, ICollection&lt; TAdded &gt; added, ICollection&lt; TRemoved &gt; removed)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::MicroServer</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</filename>
    <member kind="function">
      <type></type>
      <name>MicroServer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>a629c86701b9cb87cb08dfb32e0a69d25</anchor>
      <arglist>([Localizable(false)] string resourceName, Stream fileContent)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>a7a1cfee2dd0b06de126eecabd8febd8d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>Uri</type>
      <name>ServerUri</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>a1bab6a7639d307a2bd1dfdbe37ebd738</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Uri</type>
      <name>FileUri</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>a5696d33d6248a347646f5c4f7f127587</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Stream</type>
      <name>FileContent</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>a3b8057ff81e2ecb00eafab1baff73194</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>Slow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_micro_server.html</anchorfile>
      <anchor>aa8be846ced42e98b9c79fab299e51ec6</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::ModelViewSync</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>ModelViewSync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>a45cb213832144689fea1d6b56b38aaf5</anchor>
      <arglist>(MonitoredCollection&lt; TModel &gt; model, ICollection&lt; TView &gt; view)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Initialize</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>a88b80225cc2164bc9e724e5a7133e1e2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>TModel</type>
      <name>Lookup</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>a15b50a0d91bf79e467a40787de9daec2</anchor>
      <arglist>(TView representation)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>RegisterMultiple&lt; TSpecificModel, TSpecificView &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>ae5df80d17a40bd79b7e654eeed01d341</anchor>
      <arglist>(Func&lt; TSpecificModel, IEnumerable&lt; TSpecificView &gt;&gt; create, Action&lt; TSpecificModel, TSpecificView &gt;? update=null)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Register&lt; TSpecificModel, TSpecificView &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>a8bb6d482e9d7f33ac9cf219504f20910</anchor>
      <arglist>(Func&lt; TSpecificModel, TSpecificView &gt; create, Action&lt; TSpecificModel, TSpecificView &gt;? update=null)</arglist>
    </member>
    <member kind="property">
      <type>IEnumerable&lt; TView &gt;</type>
      <name>Representations</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_model_view_sync.html</anchorfile>
      <anchor>a9f2cf100631c34f1df4febd8ed5475f4</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::MonitoredCollection</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>MonitoredCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a384a02ce73a56eef29138b87a26f18ab</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>MonitoredCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a5591b5d893accabe77d60cb573e0ed24</anchor>
      <arglist>(int maxElements)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>AddMany</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>aac76714ec4d93f6226c67cc2b6301f74</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; collection)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>SetMany</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a1812e371b5c51407e553db0160180d42</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; enumeration)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>InsertItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a1790aab2231ec3db7c9dbc7598a6cf51</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>SetItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a43a0994ae946ac234203402ecd299a5b</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>RemoveItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a26b1efa01cd747fb85c24018af5a2754</anchor>
      <arglist>(int index)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>ClearItems</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad9d6dc34463b219fbf36f0095760db61</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>MaxElements</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad4fb3e3c71f475b673a95995f0d3a9e3</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>Changed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad9c1b9fc2add276d73caf3d19368bc50</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Added</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a91dc7c8c00dfd26fadd7bd45b0aeba5d</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Removing</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad00d4ccab9fd07229baf5cc1482fd0c2</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Removed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a9a9d6fa0fcb1f7b5be26ffa290d5b320</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>MonitoredCollection&lt; TModel &gt;</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</filename>
    <member kind="function">
      <type></type>
      <name>MonitoredCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a384a02ce73a56eef29138b87a26f18ab</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>MonitoredCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a5591b5d893accabe77d60cb573e0ed24</anchor>
      <arglist>(int maxElements)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>AddMany</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>aac76714ec4d93f6226c67cc2b6301f74</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; collection)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>SetMany</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a1812e371b5c51407e553db0160180d42</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; enumeration)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>InsertItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a1790aab2231ec3db7c9dbc7598a6cf51</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>SetItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a43a0994ae946ac234203402ecd299a5b</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>RemoveItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a26b1efa01cd747fb85c24018af5a2754</anchor>
      <arglist>(int index)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>ClearItems</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad9d6dc34463b219fbf36f0095760db61</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>MaxElements</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad4fb3e3c71f475b673a95995f0d3a9e3</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action?</type>
      <name>Changed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad9c1b9fc2add276d73caf3d19368bc50</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Added</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a91dc7c8c00dfd26fadd7bd45b0aeba5d</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Removing</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>ad00d4ccab9fd07229baf5cc1482fd0c2</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>Removed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_monitored_collection.html</anchorfile>
      <anchor>a9a9d6fa0fcb1f7b5be26ffa290d5b320</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::MoveDirectory</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_move_directory.html</filename>
    <base>NanoByte::Common::Storage::CopyDirectory</base>
    <member kind="function">
      <type></type>
      <name>MoveDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_move_directory.html</anchorfile>
      <anchor>a0dc34e83bada77f2a836acb305ad2956</anchor>
      <arglist>(string sourcePath, string destinationPath, bool preserveDirectoryTimestamps=true, bool overwrite=false)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_move_directory.html</anchorfile>
      <anchor>a548f08d8be5f7bc0efad8a4f13bdae37</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>CopyFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_move_directory.html</anchorfile>
      <anchor>aea50589e3c5f91d0879bc411a4ba146a</anchor>
      <arglist>(FileInfo sourceFile, FileInfo destinationFile)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::Msg</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Inform</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>a2a26f7ce369a1f4280757794cfd58168</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>OkCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>adeeb260eb7035da81b9206871a8ef310</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string okCaption, [Localizable(true)] string? cancelCaption=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>OkCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>a6a9b0a4e436178fbd27a5ff7d206f380</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>YesNo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>aeb8a9cba61aad4a69679d2010a221e5e</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string yesCaption, [Localizable(true)] string noCaption)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>YesNo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>a3c067bc0db659ce373abdfe55839649b</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static DialogResult</type>
      <name>YesNoCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>a920b3419dad5b1a257d9d4e7a0027f4c</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity, [Localizable(true)] string yesCaption, [Localizable(true)] string noCaption)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static DialogResult</type>
      <name>YesNoCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_msg.html</anchorfile>
      <anchor>a292fa85ad653c0f21aa018f4db10750f</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string text, MsgSeverity severity)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::MultiDictionary</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>a746d52deddecce2f2efcd7180e021d2b</anchor>
      <arglist>(TKey key, TValue value)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>af041d3c13b55fb4be5a89180694bde88</anchor>
      <arglist>(TKey key, TValue value)</arglist>
    </member>
    <member kind="property">
      <type>new IEnumerable&lt; TValue &gt;</type>
      <name>Values</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>a54ee9d6393311cbd73363819f5e3008c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>new IEnumerable&lt; TValue &gt;</type>
      <name>this[TKey key]</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>aae75074507e523ef4e0f792a89fe7083</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>MultiDictionary&lt; TModel, TView &gt;</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>a746d52deddecce2f2efcd7180e021d2b</anchor>
      <arglist>(TKey key, TValue value)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>af041d3c13b55fb4be5a89180694bde88</anchor>
      <arglist>(TKey key, TValue value)</arglist>
    </member>
    <member kind="property">
      <type>new IEnumerable&lt; TValue &gt;</type>
      <name>Values</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>a54ee9d6393311cbd73363819f5e3008c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>new IEnumerable&lt; TValue &gt;</type>
      <name>this[TKey key]</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_multi_dictionary.html</anchorfile>
      <anchor>aae75074507e523ef4e0f792a89fe7083</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::MultiPropertyChangedCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_changed_command.html</filename>
    <base>NanoByte::Common::Undo::PreExecutedCommand</base>
    <member kind="function">
      <type></type>
      <name>MultiPropertyChangedCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_changed_command.html</anchorfile>
      <anchor>a6ef27e1eb3c6751f3fe5b53442d1dc8f</anchor>
      <arglist>(object[] targets, PropertyDescriptor property, object?[] oldValues, object? newValue)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>MultiPropertyChangedCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_changed_command.html</anchorfile>
      <anchor>a8b56b19626e73d8e959d9eb807c55982</anchor>
      <arglist>(object[] targets, GridItem gridItem, object?[] oldValues)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnRedo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_changed_command.html</anchorfile>
      <anchor>a2103757e048c36500bf81b58a67329ca</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_changed_command.html</anchorfile>
      <anchor>a4d0e860f3f2c9379497ab3305bd78658</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::MultiPropertyTracker</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_tracker.html</filename>
    <member kind="function">
      <type></type>
      <name>MultiPropertyTracker</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_tracker.html</anchorfile>
      <anchor>a990c4f94f0ae7c68b5c2dc31cf590901</anchor>
      <arglist>(PropertyGrid propertyGrid)</arglist>
    </member>
    <member kind="function">
      <type>IUndoCommand</type>
      <name>GetCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_multi_property_tracker.html</anchorfile>
      <anchor>a2aad8462e124cc20d1422779fb3a52bf</anchor>
      <arglist>(GridItem changedItem)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::MutexLock</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_mutex_lock.html</filename>
    <member kind="function">
      <type></type>
      <name>MutexLock</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_mutex_lock.html</anchorfile>
      <anchor>a8a0cbfb0c349a2faad6d8c837800fdeb</anchor>
      <arglist>(string name)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_mutex_lock.html</anchorfile>
      <anchor>ab1189bcd0cdfd62064ee9bd603faa2b0</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Named</name>
    <filename>class_nano_byte_1_1_common_1_1_named.html</filename>
    <member kind="variable" static="yes">
      <type>const char</type>
      <name>TreeSeparator</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_named.html</anchorfile>
      <anchor>ae01fa2290b3cfddbd166c59888b12ad3</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::NamedCollection</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>NamedCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a3ff82ae625a7c1d37ae4056492fe654e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>NamedCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a24007a0b3d65869b982175dc66244177</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; T &gt; elements)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Rename</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>af2ee0900b5d8c7813b9eea32374e595c</anchor>
      <arglist>(T element, [Localizable(false)] string newName)</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual NamedCollection&lt; T &gt;</type>
      <name>Clone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a2e6096ac65f807b42a7ad96f4bc7d667</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override string</type>
      <name>GetKeyForItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a02209fa8197f9d7edec88a4cfa9fc45a</anchor>
      <arglist>(T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>InsertItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>af6d732aa4f5170de2e2e451080deece7</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>SetItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a57c6764bf235187f4c5c431aaab10c59</anchor>
      <arglist>(int index, T item)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>RemoveItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a6f6b0688630a323e978bb60b890284dc</anchor>
      <arglist>(int index)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>ClearItems</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_named_collection.html</anchorfile>
      <anchor>a80c043cc06c6206138ed70dba8d37d7d</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::NetUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_net_utils.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ApplyProxy</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_net_utils.html</anchorfile>
      <anchor>aff4f3e9aac26ab198df2df2402b29379</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>ConfigureTls</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_net_utils.html</anchorfile>
      <anchor>a74f013747cf35e19f5337a730848ba93</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>TrustCertificates</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_net_utils.html</anchorfile>
      <anchor>aab2f661111d9e7366922ab16e211e9d8</anchor>
      <arglist>(params string[] publicKeys)</arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsInternetConnected</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_net_utils.html</anchorfile>
      <anchor>a06c9f2cafed3388bad1a2b2d735a1520</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::NotAdminException</name>
    <filename>class_nano_byte_1_1_common_1_1_not_admin_exception.html</filename>
    <member kind="function">
      <type></type>
      <name>NotAdminException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_not_admin_exception.html</anchorfile>
      <anchor>aeff881e3a085269b20447effef5ff7e5</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>NotAdminException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_not_admin_exception.html</anchorfile>
      <anchor>a95fb1d2814cfcfe2285a917b5fde01e4</anchor>
      <arglist>(string message, Exception inner)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>NotAdminException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_not_admin_exception.html</anchorfile>
      <anchor>aaf2db3ce5fb420e7a7289e1d4eb9bdf0</anchor>
      <arglist>(string message)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type></type>
      <name>NotAdminException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_not_admin_exception.html</anchorfile>
      <anchor>a467cb5e493fc78b9dde51c31efdf359e</anchor>
      <arglist>(SerializationInfo info, StreamingContext context)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::OffsetStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</filename>
    <base>NanoByte::Common::Streams::DelegatingStream</base>
    <member kind="function">
      <type></type>
      <name>OffsetStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</anchorfile>
      <anchor>a3f5d17cdaf896b2c5f589e1c3dc5d4e4</anchor>
      <arglist>(Stream underlyingStream, long offset)</arglist>
    </member>
    <member kind="function">
      <type>override long</type>
      <name>Seek</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</anchorfile>
      <anchor>af74a8bd4c6c46b12e13d4eb37a25c235</anchor>
      <arglist>(long offset, SeekOrigin origin)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>SetLength</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</anchorfile>
      <anchor>a35cc1887175a44078bec7fa67978bf1a</anchor>
      <arglist>(long value)</arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Length</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</anchorfile>
      <anchor>a39bdb8c12704121e2fb2e0c6aae8e66e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Position</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_offset_stream.html</anchorfile>
      <anchor>a5da350202a1cafdbd2024e21dca1883c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::OKCancelDialog</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_o_k_cancel_dialog.html</filename>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>OnOKClicked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_o_k_cancel_dialog.html</anchorfile>
      <anchor>aeb17d3fb67cf2ccdcc3b2601507d50a3</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual void</type>
      <name>OnCancelClicked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_o_k_cancel_dialog.html</anchorfile>
      <anchor>a195db8944143664a6d1031f653f8fe2a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_o_k_cancel_dialog.html</anchorfile>
      <anchor>a533040a5fd3c2c3c1278de3d16a430d3</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="struct">
    <name>NanoByte::Common::Info::OSInfo</name>
    <filename>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</filename>
    <member kind="variable">
      <type>string</type>
      <name>Platform</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>acbe94413687b811e9f087c095fff3edb</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>string</type>
      <name>Version</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>a4eec010bc7811d1e2d1d02f366a39a40</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>string</type>
      <name>ServicePack</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>a5a9b747a18edc503f87b2f736fafbd06</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>string</type>
      <name>FrameworkVersion</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>a01c35d799eaf552169cda89dab262d55</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Architecture</type>
      <name>OSArchitecture</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>a73bd5434030c4bc8dda8607f27e6a857</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Architecture</type>
      <name>ProcessArchitecture</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>ace44f110358bf47a25dd6f43078596a8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static OSInfo</type>
      <name>Current</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_info_1_1_o_s_info.html</anchorfile>
      <anchor>a61d34c9c8585c3e84f2ff986c0575f34</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::OSUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_o_s_utils.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ExpandVariables</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_o_s_utils.html</anchorfile>
      <anchor>a059455600552ff4b854fcc16d2aff242</anchor>
      <arglist>(string value, IDictionary&lt; string, string &gt; variables)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ExpandVariables</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_o_s_utils.html</anchorfile>
      <anchor>aa0c961fe7fd2978f5afd411bd3524441</anchor>
      <arglist>(string value, StringDictionary variables)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::OutputBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_output_box.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Show</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_output_box.html</anchorfile>
      <anchor>a2ef53ec5717806e36bd3f4b03e199730</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string title, [Localizable(true)] string message)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_output_box.html</anchorfile>
      <anchor>a5d49f78929c284d76e245c3997760bfd</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::OutputGridBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_output_grid_box.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Show&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_output_grid_box.html</anchorfile>
      <anchor>a8b0ecb9e0374d6ec6f8d7172b91f4d83</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_output_grid_box.html</anchorfile>
      <anchor>a3de491c7a3f6d1ad79ff59505b773c51</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::OutputTreeBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_output_tree_box.html</filename>
    <templarg></templarg>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Show&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_output_tree_box.html</anchorfile>
      <anchor>a23922f47459aa80adeedfb265154a097</anchor>
      <arglist>(IWin32Window? owner, [Localizable(true)] string title, NamedCollection&lt; T &gt; data, char separator=Named.TreeSeparator)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::Paths</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_paths.html</filename>
    <member kind="function" static="yes">
      <type>static IList&lt; FileInfo &gt;</type>
      <name>ResolveFiles</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_paths.html</anchorfile>
      <anchor>a7747e265a53ee952ee4708f08ab2f472</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; string &gt; paths, [Localizable(false)] string defaultPattern=&quot;*&quot;)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Dispatch::PerTypeDispatcher</name>
    <filename>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>PerTypeDispatcher</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>a57388f8bc2a955f7eab8417b5858a58f</anchor>
      <arglist>(bool ignoreMissing)</arglist>
    </member>
    <member kind="function">
      <type>PerTypeDispatcher&lt; TBase &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>ae8f0a43ad4dfc88a3f318002fb273d4f</anchor>
      <arglist>(Action&lt; TSpecific &gt; action)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>a62426b3df59f27bae2a3ffa0d272cc5b</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>a720b471e5134fb20001117533c6e1b48</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
    <member kind="function">
      <type>PerTypeDispatcher&lt; TBase, TResult &gt;</type>
      <name>Add&lt; TSpecific &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>a0a97ec2caa52e95b9d2c7f1f567dc8e6</anchor>
      <arglist>(Func&lt; TSpecific, TResult &gt; function)</arglist>
    </member>
    <member kind="function">
      <type>TResult</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>ae674e1c6d156a4ac50b3158fb62a24d5</anchor>
      <arglist>(TBase element)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; TResult &gt;</type>
      <name>Dispatch</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_dispatch_1_1_per_type_dispatcher.html</anchorfile>
      <anchor>a20b3802f462e94f7ed06f3ee85fdc65b</anchor>
      <arglist>([InstantHandle] IEnumerable&lt; TBase &gt; elements)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::PreExecutedCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_command.html</filename>
    <base>NanoByte::Common::Undo::FirstExecuteCommand</base>
    <member kind="function" protection="protected">
      <type>sealed override void</type>
      <name>OnFirstExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_command.html</anchorfile>
      <anchor>a3554e0d4301fc5317aa569538e1f2e09</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::PreExecutedCompositeCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_composite_command.html</filename>
    <base>NanoByte::Common::Undo::PreExecutedCommand</base>
    <member kind="function">
      <type></type>
      <name>PreExecutedCompositeCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_composite_command.html</anchorfile>
      <anchor>acb39fcf0a20f340c887c0439ae97d1c5</anchor>
      <arglist>(IEnumerable&lt; IUndoCommand &gt; commands)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnRedo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_composite_command.html</anchorfile>
      <anchor>aaf23a42712aedbae5d86b40ce8f4709e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_pre_executed_composite_command.html</anchorfile>
      <anchor>ac99f68e0912f1f5fe42353b89e035675</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::ProcessUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_process_utils.html</filename>
    <member kind="function" static="yes">
      <type>static Process</type>
      <name>Start</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a2adb10a0a0b32ab92e949b59d4993eaa</anchor>
      <arglist>(this ProcessStartInfo startInfo)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Process</type>
      <name>Start</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a93c43493a160dbb0d4b244f5432ec46f</anchor>
      <arglist>(string fileName, params string[] arguments)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>Run</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a1a3e8696fc7075083127c1416d8f42b6</anchor>
      <arglist>(this ProcessStartInfo startInfo)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ProcessStartInfo</type>
      <name>Assembly</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a8f2ece3e0db1af3742c080a0b07a68ee</anchor>
      <arglist>(string name, params string[] arguments)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ProcessStartInfo</type>
      <name>AsAdmin</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a34e187eff81acbef937b5cab6bff529b</anchor>
      <arglist>(this ProcessStartInfo startInfo)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SanitizeEnvironmentVariables</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>a59a75db4f2cce01b155d4c184309c1f6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>JoinEscapeArguments</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>ad5c4db3d16100ac8674589c758826b3e</anchor>
      <arglist>(this IEnumerable&lt; string &gt; parts)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>EscapeArgument</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_process_utils.html</anchorfile>
      <anchor>af3d72b1340279275a9aa95299c5e6f17</anchor>
      <arglist>(this string value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::ProducerConsumerStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</filename>
    <member kind="function">
      <type></type>
      <name>ProducerConsumerStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a2671e597f608cc641b0bf7f7b6c53112</anchor>
      <arglist>(int bufferSize=163840)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>SetLength</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>afce6efc90b65509f57367cc13edde070</anchor>
      <arglist>(long value)</arglist>
    </member>
    <member kind="function">
      <type>override long</type>
      <name>Seek</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>ab82d249fab07a3e8dfbede46f9089796</anchor>
      <arglist>(long offset, SeekOrigin origin)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Flush</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a1a784e97ed5f43b23b0b6b6cb28cb45b</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a39f06748a54d130d38137bf8a2b5520f</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>ac628aee8a765da0e8340b52fbcd23291</anchor>
      <arglist>(Span&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>CopyTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>aa21d76fecbd5763821602f2b09e2b3d9</anchor>
      <arglist>(Stream destination, int bufferSize)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a14b328de30dec81ce39345216ce324fc</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a556b3fa38e917389489d2d8ebdab78a5</anchor>
      <arglist>(ReadOnlySpan&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>RelayErrorToReader</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a20dd8e8c3bbcf0a46f41c97004c92155</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>DoneWriting</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>addcf357a946b5ff039495528a71684bb</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a21edc9fdc81e151be6ab6db1bbe5985d</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanRead</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a1281a85c6b8de636d3f672661d9e9854</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanWrite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a9695e663ae33b34b9b6bd3e1d8b8e825</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanSeek</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>acf4d79c04fbe8a866432dd9340a578a6</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Position</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>ab7786eccb9124f18ca4fe408cea70403</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>long</type>
      <name>PositionWrite</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>aab46f960f4d67af212cc407abfa21d0b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Length</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_producer_consumer_stream.html</anchorfile>
      <anchor>a88c373340a076bed7eb806b6c91710d8</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::ProgressStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</filename>
    <base>NanoByte::Common::Streams::DelegatingStream</base>
    <member kind="function">
      <type></type>
      <name>ProgressStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>af317a27fc958f3eaaed2f706ac026388</anchor>
      <arglist>(Stream underlyingStream, IProgress&lt; long &gt;? progress=null, CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>SetLength</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>ab9dcd3083e15bac1e46e0cd5a2506167</anchor>
      <arglist>(long value)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a06ee2edee1dd10a7e055b5517d9cd14b</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a0c8b396f5a8b0f1bdcc0e8fa99226541</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override async Task&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>aee09d084a7b8a0b960d43eab59e5a183</anchor>
      <arglist>(byte[] buffer, int offset, int count, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override async Task</type>
      <name>WriteAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a5884d6e62fc44118ff09c9a33e7962a5</anchor>
      <arglist>(byte[] buffer, int offset, int count, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a72650a24be1e16fb67bce29f46767587</anchor>
      <arglist>(Span&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>override async ValueTask&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a567f18877a85efc67e939df1c2c9f943</anchor>
      <arglist>(Memory&lt; byte &gt; buffer, CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>af8bcbbee2600844a2899ffe7ce9653b3</anchor>
      <arglist>(ReadOnlySpan&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function">
      <type>override async ValueTask</type>
      <name>WriteAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a6c6b1522d9c30b8bdc759bf1b16f1708</anchor>
      <arglist>(ReadOnlyMemory&lt; byte &gt; buffer, CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="property">
      <type>override long</type>
      <name>Length</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_progress_stream.html</anchorfile>
      <anchor>a9ccca0c1cbd3d4b2143e548dedae181f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::PropertyChangedCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_property_changed_command.html</filename>
    <base>NanoByte::Common::Undo::PreExecutedCommand</base>
    <member kind="function">
      <type></type>
      <name>PropertyChangedCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_property_changed_command.html</anchorfile>
      <anchor>ae41860c63ee4c96146e25a5833f5af58</anchor>
      <arglist>(object target, PropertyDescriptor property, object? oldValue, object? newValue)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>PropertyChangedCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_property_changed_command.html</anchorfile>
      <anchor>adb4873688303ef7bacae6c988ace204c</anchor>
      <arglist>(object target, PropertyValueChangedEventArgs e)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnRedo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_property_changed_command.html</anchorfile>
      <anchor>a2903dcf48615422a2101d92e921cb280</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_property_changed_command.html</anchorfile>
      <anchor>af982502c75e165a238e53f18da5d23df</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::PropertyGridForm</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_property_grid_form.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Show</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_property_grid_form.html</anchorfile>
      <anchor>af5805c68c50e2401fdaf5bc8f442d30d</anchor>
      <arglist>(object value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::PropertyPointer</name>
    <filename>class_nano_byte_1_1_common_1_1_property_pointer.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>PropertyPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>acf697bc4628c8b1b61232066a76e121e</anchor>
      <arglist>(Func&lt; T &gt; getValue, Action&lt; T &gt; setValue, T defaultValue, bool needsEncoding=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a3b8dfc08c38ad5a673b142f66c8883ae</anchor>
      <arglist>(Func&lt; T &gt; getValue, Action&lt; T &gt; setValue, T defaultValue, bool needsEncoding=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; T?&gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a9486f540fe3be3c24e44f00fa5272fdc</anchor>
      <arglist>(Func&lt; T?&gt; getValue, Action&lt; T?&gt; setValue, bool needsEncoding=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a2e96ef1d91b93e7d60f3a3a69c394ac7</anchor>
      <arglist>(Expression&lt; Func&lt; T &gt;&gt; getValue, T defaultValue, bool needsEncoding=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; T?&gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a622b41561af737d03e56a6f7f451541a</anchor>
      <arglist>(Expression&lt; Func&lt; T?&gt;&gt; getValue, bool needsEncoding=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; string &gt;</type>
      <name>ToStringPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a63db272b05d16e5f55272a2511ab9766</anchor>
      <arglist>(this PropertyPointer&lt; bool &gt; pointer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; string &gt;</type>
      <name>ToStringPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a5189703baeb63fa1ba1781de03848105</anchor>
      <arglist>(this PropertyPointer&lt; int &gt; pointer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; string &gt;</type>
      <name>ToStringPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a273b991a97e70dcca072558e0967226f</anchor>
      <arglist>(this PropertyPointer&lt; long &gt; pointer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; string &gt;</type>
      <name>ToStringPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>af4fcc393855efd6d0f971c41b40e0b1d</anchor>
      <arglist>(this PropertyPointer&lt; TimeSpan &gt; pointer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static PropertyPointer&lt; string &gt;</type>
      <name>ToStringPointer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>af2f6c68a850b4525075881070c44e9f1</anchor>
      <arglist>(this PropertyPointer&lt; Uri?&gt; pointer)</arglist>
    </member>
    <member kind="property">
      <type>T</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a385c469057090295e5966311bbf220c8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>T</type>
      <name>DefaultValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>af679d59dcc61a268ff06e6cdc73c5df2</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsDefaultValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a4e1181b739f90ff7e989e5027c5f0147</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>NeedsEncoding</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_property_pointer.html</anchorfile>
      <anchor>a1332f0737adc3c2feb7c1a38f7b7a86b</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::ReadFile</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>ReadFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</anchorfile>
      <anchor>af7428651f3d441ff0510fc777429fde7</anchor>
      <arglist>(string path, Action&lt; Stream &gt; callback)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</anchorfile>
      <anchor>a99bfa2afcb20161e20bda7d9637a4603</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</anchorfile>
      <anchor>aeb8ccb30939a5c7c7bfde55bd86e1b75</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</anchorfile>
      <anchor>a903bf421a116f7d85191b4ddbc6b48c5</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_read_file.html</anchorfile>
      <anchor>a1a5394e1015a27b20746913e7178c617</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::RegistryUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</filename>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>GetDword</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a1f9397ae72ad2cca1107d906017eacc4</anchor>
      <arglist>([Localizable(false)] string keyName, [Localizable(false)] string? valueName, int defaultValue=0)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetDword</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a3e2a9ed983bea3b40427c12325655433</anchor>
      <arglist>([Localizable(false)] string keyName, [Localizable(false)] string? valueName, int value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>GetString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>ae77d440d7762c3bd9decd42f2b175af8</anchor>
      <arglist>([Localizable(false)] string keyName, [Localizable(false)] string? valueName, [Localizable(false)] string? defaultValue=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a59bcf4ff167127188a385ac14c5244a9</anchor>
      <arglist>([Localizable(false)] string keyName, [Localizable(false)] string? valueName, [Localizable(false)] string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>GetSoftwareString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a0cf906e22b417c72dda92349f5842484</anchor>
      <arglist>([Localizable(false)] string subkeyName, [Localizable(false)] string? valueName, [Localizable(false)] string? defaultValue=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>GetSoftwareString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>ae199d514c6037656769200657d1a0724</anchor>
      <arglist>([Localizable(false)] string subkeyName, [Localizable(false)] string? valueName, bool machineWide)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetSoftwareString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>aaf213147dd46120dc8a4df57c5e9ca60</anchor>
      <arglist>([Localizable(false)] string subkeyName, [Localizable(false)] string? valueName, [Localizable(false)] string value, bool machineWide=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>DeleteSoftwareValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>acb60aee5a2d71f88ad836890d101f8d9</anchor>
      <arglist>([Localizable(false)] string subkeyName, [Localizable(false)] string valueName, bool machineWide)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string[]</type>
      <name>GetValueNames</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>aa406d2a60dc1a02b6fd7011d76ea488e</anchor>
      <arglist>([Localizable(false)] this RegistryKey key, [Localizable(false)] string subkeyName)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string[]</type>
      <name>GetSubKeyNames</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a47911a491e45120a7168615947dda048</anchor>
      <arglist>([Localizable(false)] RegistryKey key, [Localizable(false)] string subkeyName)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static RegistryKey</type>
      <name>OpenSubKeyChecked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a34245987d52ffef40bcdc8fecdfe4e10</anchor>
      <arglist>([Localizable(false)] this RegistryKey key, [Localizable(false)] string subkeyName, bool writable=false)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static RegistryKey</type>
      <name>CreateSubKeyChecked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a3344d54e6a8b33415ae41f120fdb06a4</anchor>
      <arglist>([Localizable(false)] this RegistryKey key, [Localizable(false)] string subkeyName)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static RegistryKey</type>
      <name>OpenHklmKey</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_registry_utils.html</anchorfile>
      <anchor>a8dcf87b67123d1450cf9af10add73023</anchor>
      <arglist>([Localizable(false)] string subkeyName, out bool x64)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::RemoveFromCollection</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_remove_from_collection.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::CollectionCommand</base>
    <member kind="function">
      <type></type>
      <name>RemoveFromCollection</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_remove_from_collection.html</anchorfile>
      <anchor>a39c6d8267857d6bba80d4ee35bca46d4</anchor>
      <arglist>(ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static RemoveFromCollection&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_remove_from_collection.html</anchorfile>
      <anchor>a7e111843723332352847ff09fccacc51</anchor>
      <arglist>(ICollection&lt; T &gt; collection, T element)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_remove_from_collection.html</anchorfile>
      <anchor>ad03051cfe22e6cc0773228a8e897bfbb</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_remove_from_collection.html</anchorfile>
      <anchor>a2ae5328bd4ab5bd4a9a61edc9c47697f</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::ReplaceInList</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <base>NanoByte::Common::Undo::IValueCommand</base>
    <member kind="function">
      <type></type>
      <name>ReplaceInList</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</anchorfile>
      <anchor>a832103810b22c5e885e80641bbed8c00</anchor>
      <arglist>(IList&lt; T &gt; list, T oldElement, T newElement)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ReplaceInList&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</anchorfile>
      <anchor>a82d43d78eec585a89b61ec5a2e536fc6</anchor>
      <arglist>(IList&lt; T &gt; list, T oldElement, T newElement)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</anchorfile>
      <anchor>ae93fafa4fec7e0e90783938beda112a8</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</anchorfile>
      <anchor>a2df6605ca13647828493655e1358a5d5</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>object</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_replace_in_list.html</anchorfile>
      <anchor>a4081491ea2ef918bd31784db6f7699ca</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::ResettablePropertyGrid</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_resettable_property_grid.html</filename>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::RtfBuilder</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_rtf_builder.html</filename>
    <member kind="function">
      <type>void</type>
      <name>AppendPar</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_rtf_builder.html</anchorfile>
      <anchor>afc230718ac1228861402e4f330f6b13c</anchor>
      <arglist>(string text, RtfColor color)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_rtf_builder.html</anchorfile>
      <anchor>a182914931710370815d2ece6843bec4e</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Drawing::ScalableImage</name>
    <filename>class_nano_byte_1_1_common_1_1_drawing_1_1_scalable_image.html</filename>
    <member kind="function">
      <type></type>
      <name>ScalableImage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_drawing_1_1_scalable_image.html</anchorfile>
      <anchor>a24dac03d1bcb40a7af3ffc2997ea62ac</anchor>
      <arglist>(Image image)</arglist>
    </member>
    <member kind="function">
      <type>Image</type>
      <name>Get</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_drawing_1_1_scalable_image.html</anchorfile>
      <anchor>a5eccfb0b9fe5b45566be3b17054f717c</anchor>
      <arglist>(float factor)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_drawing_1_1_scalable_image.html</anchorfile>
      <anchor>adb6b3b96bdfb0e0d4afeb35a86e3e365</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::ServiceTaskHandler</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_service_task_handler.html</filename>
    <base>NanoByte::Common::Tasks::SilentTaskHandler</base>
    <member kind="function">
      <type></type>
      <name>ServiceTaskHandler</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_service_task_handler.html</anchorfile>
      <anchor>a8991ae4b9e34fa338ea55bf27e1e3604</anchor>
      <arglist>(IServiceProvider provider)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_service_task_handler.html</anchorfile>
      <anchor>ac2ae56c9539a7472a9ae2d9cbb92ab1a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override? ICredentialProvider</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_service_task_handler.html</anchorfile>
      <anchor>a5172ff6454b749cc08a325726da740ef</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::SetInList</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <base>NanoByte::Common::Undo::IValueCommand</base>
    <member kind="function">
      <type></type>
      <name>SetInList</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</anchorfile>
      <anchor>a1f8c63162f7693d0c03f5e71b8904722</anchor>
      <arglist>(IList&lt; T &gt; list, T oldElement, T newElement)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static SetInList&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</anchorfile>
      <anchor>aa228604fc15f87166c210a02ae500cdd</anchor>
      <arglist>(IList&lt; T &gt; list, T oldElement, T newElement)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</anchorfile>
      <anchor>a4af15c68818c27218bac76f865a1b627</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</anchorfile>
      <anchor>acc58a793e97746c8cc0af8b8b02d6976</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>object</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_in_list.html</anchorfile>
      <anchor>a327c59d116672ca1576d509911e0785f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::SetLocalizableString</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_set_localizable_string.html</filename>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <member kind="function">
      <type></type>
      <name>SetLocalizableString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_localizable_string.html</anchorfile>
      <anchor>a0037f7074446760b7de200514dd2d58a</anchor>
      <arglist>(LocalizableStringCollection collection, LocalizableString element)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_localizable_string.html</anchorfile>
      <anchor>ab7120b4bffe89cb476592de317d67b41</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_localizable_string.html</anchorfile>
      <anchor>a07fe0485b4a7e50d7acd7c8ffa8621f9</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::SetValueCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</filename>
    <templarg></templarg>
    <base>NanoByte::Common::Undo::SimpleCommand</base>
    <base>NanoByte::Common::Undo::IValueCommand</base>
    <member kind="function">
      <type></type>
      <name>SetValueCommand</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>a79132253dc354718af55713b8be55f76</anchor>
      <arglist>(PropertyPointer&lt; T &gt; pointer, T newValue)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static SetValueCommand&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>a2be1b01e6b8940721ae59c61896efcee</anchor>
      <arglist>(PropertyPointer&lt; T &gt; pointer, T newValue)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static SetValueCommand&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>a76faf4fbc62ae2e42bd51ae7aaef21f3</anchor>
      <arglist>(Func&lt; T &gt; getValue, Action&lt; T &gt; setValue, T newValue)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static SetValueCommand&lt; T &gt;</type>
      <name>For&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>ab517d296d2684c162fe9ce2ecaea4993</anchor>
      <arglist>(Expression&lt; Func&lt; T &gt;&gt; expression, T newValue)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>abfc9c69cdaa571abb1598af3ec6fe673</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>a39cf029774413f0f9216d352a7ee8ccc</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>object?</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_set_value_command.html</anchorfile>
      <anchor>a84b5d80cc38756ac9612c5a6953129ed</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::ShadowingStream</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_shadowing_stream.html</filename>
    <base>NanoByte::Common::Streams::DelegatingStream</base>
    <member kind="function">
      <type></type>
      <name>ShadowingStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_shadowing_stream.html</anchorfile>
      <anchor>acd4849db8f2f02842c446e4f57dd7a6b</anchor>
      <arglist>(Stream underlyingStream, Stream shadowStream)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_shadowing_stream.html</anchorfile>
      <anchor>abfec45cef3eec75596e69e1828d66434</anchor>
      <arglist>(byte[] buffer, int offset, int count)</arglist>
    </member>
    <member kind="function">
      <type>override async Task&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_shadowing_stream.html</anchorfile>
      <anchor>ac3d3f86e024468ac56ed0faaf8879178</anchor>
      <arglist>(byte[] buffer, int offset, int count, CancellationToken cancellationToken)</arglist>
    </member>
    <member kind="function">
      <type>override async ValueTask&lt; int &gt;</type>
      <name>ReadAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_shadowing_stream.html</anchorfile>
      <anchor>ab5f7820de91b16992408028433c01c89</anchor>
      <arglist>(Memory&lt; byte &gt; buffer, CancellationToken cancellationToken=default)</arglist>
    </member>
  </compound>
  <compound kind="struct">
    <name>NanoByte::Common::Native::WindowsTaskbar::ShellLink</name>
    <filename>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</filename>
    <member kind="function">
      <type></type>
      <name>ShellLink</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a77fc65bddb0e46f8814e20b63db3b680</anchor>
      <arglist>([Localizable(true)] string title, [Localizable(false)] string path, [Localizable(false)] string? arguments=null)</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>ShellLink</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a1f7255429d15a38bff9f3356a285b21b</anchor>
      <arglist>([Localizable(true)] string title, [Localizable(false)] string path, [Localizable(false)] string arguments, [Localizable(false)] string iconPath, int iconIndex)</arglist>
    </member>
    <member kind="variable">
      <type>readonly string</type>
      <name>Title</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a9ad189765480272d5a4147b7d9965318</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly string</type>
      <name>Path</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a1eb0541f17e69cafa70a67d09c8dd8a4</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly? string</type>
      <name>Arguments</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a2771627af0d3a6fd226285f57530b816</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly string</type>
      <name>IconPath</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a5163d0186ae0d26433a5f7af6245103a</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>readonly int</type>
      <name>IconIndex</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar_1_1_shell_link.html</anchorfile>
      <anchor>a0bb5dde31f5c1ef6da3fba4295b41fa1</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::SilentTaskHandler</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_silent_task_handler.html</filename>
    <base>NanoByte::Common::Tasks::TaskHandlerBase</base>
    <member kind="function">
      <type>override void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_silent_task_handler.html</anchorfile>
      <anchor>a3c80da445fc4e9a1a25100576aea147e</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_silent_task_handler.html</anchorfile>
      <anchor>a6d96cb179962747cd4f2880816aa39c9</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_silent_task_handler.html</anchorfile>
      <anchor>a65f5516f5c293a9647e57c9aedf0cfba</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::SimpleCommand</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command.html</filename>
    <base>NanoByte::Common::Undo::IUndoCommand</base>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command.html</anchorfile>
      <anchor>a434f1e4a12efa899b15d6aedae9bb93a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Undo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command.html</anchorfile>
      <anchor>a27a704be43e8c7771eec9fd9087665e0</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnExecute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command.html</anchorfile>
      <anchor>a97599e66bd80c759b2bb15d95048c286</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnUndo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command.html</anchorfile>
      <anchor>a4087f5da9b7bfdba36f8fc44868929d3</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Undo::SimpleCommandExecutor</name>
    <filename>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command_executor.html</filename>
    <base>NanoByte::Common::Undo::ICommandExecutor</base>
    <member kind="function">
      <type>void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command_executor.html</anchorfile>
      <anchor>a6dedbbd7986edebbfa6c6f199cdc9fb5</anchor>
      <arglist>(IUndoCommand command)</arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_undo_1_1_simple_command_executor.html</anchorfile>
      <anchor>a4e6fa68941faed084e2af62484dd1fc7</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::SimplePercentTask</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>SimplePercentTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</anchorfile>
      <anchor>a3ec37714af1b3214328d1b2b0867e487</anchor>
      <arglist>([Localizable(true)] string name, Action&lt; PercentProgressCallback &gt; work, Action? cancellationCallback=null)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</anchorfile>
      <anchor>a23ccf18e3aea78eda25d2c23b06dd87b</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</anchorfile>
      <anchor>a5b62e6f5457045778d55ca2dbf969377</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</anchorfile>
      <anchor>a0df4126a57fe3a12b5e9f06e462841e9</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_percent_task.html</anchorfile>
      <anchor>a4cd1604917514696beae99427d61da3f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::SimpleTask</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>SimpleTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</anchorfile>
      <anchor>a49b66f170db50b2329c44487b8a1d8a6</anchor>
      <arglist>([Localizable(true)] string name, Action work, Action? cancellationCallback=null)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</anchorfile>
      <anchor>add90d62a20945a48ac24c3f1d934daf7</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</anchorfile>
      <anchor>a56e95fc79cc643018e4cc7689e0f8644</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>override bool</type>
      <name>CanCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</anchorfile>
      <anchor>abef0c5022f1d8e94b89159df338ea178</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_simple_task.html</anchorfile>
      <anchor>a3ed38641c842dc7347198455184aaf79</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::StackExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_stack_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>PopEach&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_stack_extensions.html</anchorfile>
      <anchor>a4ad252bda3f1105313cd381c1e0a5f67</anchor>
      <arglist>(this Stack&lt; T &gt; stack, [InstantHandle] Action&lt; T &gt; action)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::StagedOperation</name>
    <filename>class_nano_byte_1_1_common_1_1_staged_operation.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Stage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>a003a9d4a095f7ae39d9ac57d363e6afc</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Commit</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>ac06b20c18df0a9d0706654a9f0f57635</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>abef0137ce9e573efe07a86751f5c472e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnStage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>a274a6980af957f592a01cb80c8bf6c9d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnCommit</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>a8c34c097692453659b2c8d09b255ede2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>OnRollback</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_staged_operation.html</anchorfile>
      <anchor>ae6756e873fd9ee318e1ab49b16ceab56</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::StreamConsumer</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_stream_consumer.html</filename>
    <member kind="function">
      <type></type>
      <name>StreamConsumer</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_consumer.html</anchorfile>
      <anchor>a2b88a54c2452a9d8c05c055b1d41291b</anchor>
      <arglist>(StreamReader reader)</arglist>
    </member>
    <member kind="function">
      <type>string?</type>
      <name>ReadLine</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_consumer.html</anchorfile>
      <anchor>aac8784105afdde7e6bab684798b9a59a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>WaitForEnd</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_consumer.html</anchorfile>
      <anchor>a1c5bb50075ea565f1f3f9359baed3fdd</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_consumer.html</anchorfile>
      <anchor>a7e5d98c5610f5306d1761db03dedcfe9</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Streams::StreamUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</filename>
    <member kind="function" static="yes">
      <type>static byte[]</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ab53fd55093aab83568675c27cd8485db</anchor>
      <arglist>(this Stream stream, int count)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>Read</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>af6ef4969e0f88a24173c0da0dc7a112d</anchor>
      <arglist>(this Stream stream, ArraySegment&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? byte[]</type>
      <name>TryRead</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a78d9e84396f13d3d37111ea57a9329c6</anchor>
      <arglist>(this Stream stream, int count)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ArraySegment&lt; byte &gt;</type>
      <name>ReadAll</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ac5854464e543d68128967d4738bcc91f</anchor>
      <arglist>(this Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a17285a7b4c95bb51bff7c7360dc4e892</anchor>
      <arglist>(this Stream stream, params byte[] data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Write</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ac9df2890ee5ddcb1785b03fbf56a8c6d</anchor>
      <arglist>(this Stream stream, ArraySegment&lt; byte &gt; buffer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static byte[]</type>
      <name>AsArray</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a7bb0cc81ef2350ae1a745eb9bee40a9e</anchor>
      <arglist>(this Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static MemoryStream</type>
      <name>ToMemory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a256aa787a44d9dd32b97897f9c2c54a3</anchor>
      <arglist>(this Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CopyToEx</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ab64263868baf90f5322233d4dc5b5944</anchor>
      <arglist>(this Stream source, Stream destination, int bufferSize=81920)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CopyToFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ab311a45438de9a6d6348bb558aa0ea12</anchor>
      <arglist>(this Stream stream, [Localizable(false)] string path, int bufferSize=81920)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ReadToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>abaaec84bd6f74e9c6f133e84d5e905ed</anchor>
      <arglist>(this Stream stream, Encoding? encoding=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static MemoryStream</type>
      <name>ToStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a8b14cd0d89adf2bbea902d0e4e91f2b4</anchor>
      <arglist>(this string data, Encoding? encoding=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Stream</type>
      <name>GetEmbeddedStream</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a4c043fe92542a83ec5b38b67f42fff52</anchor>
      <arglist>(this Type type, [Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static byte[]</type>
      <name>GetEmbeddedBytes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a0ac65a15ea45b8880d052a2c04dc4dff</anchor>
      <arglist>(this Type type, [Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetEmbeddedString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>a50b6c99c7d642060dd43e7cc7fa8e6d2</anchor>
      <arglist>(this Type type, [Localizable(false)] string name, Encoding? encoding=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CopyEmbeddedToFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_streams_1_1_stream_utils.html</anchorfile>
      <anchor>ace61d283441eaa22eb422b45a92dfc18</anchor>
      <arglist>(this Type type, [Localizable(false)] string name, [Localizable(false)] string path)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::StringConstructorConverter</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_string_constructor_converter.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>override bool</type>
      <name>CanConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_string_constructor_converter.html</anchorfile>
      <anchor>af32e02c8984cefd6afe6df6386c220c2</anchor>
      <arglist>(ITypeDescriptorContext context, Type sourceType)</arglist>
    </member>
    <member kind="function">
      <type>override? object</type>
      <name>ConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_string_constructor_converter.html</anchorfile>
      <anchor>a27ba21796f406f5817a0ba3e80b64aa5</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value)</arglist>
    </member>
    <member kind="function">
      <type>override? object</type>
      <name>ConvertTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_string_constructor_converter.html</anchorfile>
      <anchor>a1535069fec8122c05e668dee0d8ccf58</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::StringUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_string_utils.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EqualsIgnoreCase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a439069a94baec7eef10a9e618d61b9ac</anchor>
      <arglist>(string? s1, string? s2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EqualsIgnoreCase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>abd1b342bfc17faab8501d3b6c5ba1850</anchor>
      <arglist>(char c1, char c2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EqualsEmptyNull</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>ade246df2bc15e08f37f7059ffac0f31c</anchor>
      <arglist>(string? s1, string? s2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ContainsIgnoreCase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a2c93c79ab3aa7be62fd89f095544b249</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ContainsWhitespace</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a92e6ecf83eb6787c14728250d356953e</anchor>
      <arglist>(this string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>StartsWith</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a4088ad9b64dd95a37928b8fba26d4a66</anchor>
      <arglist>(this string value, string searchFor, [NotNullWhen(true)] out string? rest)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EndsWith</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a3c321e0645a20594d799a8aeb26d47ce</anchor>
      <arglist>(this string value, string searchFor, [NotNullWhen(true)] out string? rest)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>StartsWithIgnoreCase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a2a03d1f6299efad1e2c04a0153fb8350</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>EndsWithIgnoreCase</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a938c4a3378f1493a8b9a5174608a08f4</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>RemoveCharacters</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>ae0d603215e0f8bd7d7d647acba431133</anchor>
      <arglist>(this string? value, [InstantHandle] IEnumerable&lt; char &gt; characters)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>TrimOverflow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a63c93e6e5a16d3f2a0b6592cdb2ebfb1</anchor>
      <arglist>(this string value, int maxLength)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string[]</type>
      <name>SplitMultilineText</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>aa1605aaaa496ce74c39017c17e241405</anchor>
      <arglist>(this string value)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>Join</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a886acab372f0b4e96a2a170cbf160d21</anchor>
      <arglist>(string separator, [InstantHandle] IEnumerable&lt; string &gt; parts)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetLeftPartAtFirstOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>ad666899de96259e2665e76336e54edce</anchor>
      <arglist>(this string value, char searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetRightPartAtFirstOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a6b372073003cb59eaa4aceb5441e67ff</anchor>
      <arglist>(this string value, char searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetLeftPartAtLastOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a7d8989acdf3d04c3f0ca24fab798f797</anchor>
      <arglist>(this string value, char searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetRightPartAtLastOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a7ed017a760b4f6bc5115171ecb5c55ca</anchor>
      <arglist>(this string value, char searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetLeftPartAtFirstOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a941456da5025777a8effba23a88ef114</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetRightPartAtFirstOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a95b0ecacfa80ed29d5c5fd5cc4669e87</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetLeftPartAtLastOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a79f4bf2cfee616882bfc161ba6000fcc</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetRightPartAtLastOccurrence</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>a9c66a5f23ff8eee136e75d276df73800</anchor>
      <arglist>(this string value, string searchFor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>FormatBytes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>abac64d43251effc873734798edfd7647</anchor>
      <arglist>(this long value, IFormatProvider? provider=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GeneratePassword</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_string_utils.html</anchorfile>
      <anchor>aa449ef87d7b416d2679674010577c021</anchor>
      <arglist>(int length)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::SynchronousProgress</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_synchronous_progress.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type></type>
      <name>SynchronousProgress</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_synchronous_progress.html</anchorfile>
      <anchor>a7df42176ad6e09942792e372935f17c3</anchor>
      <arglist>(Action&lt; T &gt;? callback=null)</arglist>
    </member>
    <member kind="event">
      <type>Action&lt; T &gt;?</type>
      <name>ProgressChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_synchronous_progress.html</anchorfile>
      <anchor>a72c4150add7d4ff51fdcf3a382d30595</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::TaskBase</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</filename>
    <base>NanoByte::Common::Tasks::ITask</base>
    <member kind="function">
      <type>void</type>
      <name>Run</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a42eb3238bc9f0bee8df0ef6a4e45aaef</anchor>
      <arglist>(CancellationToken cancellationToken=default, ICredentialProvider? credentialProvider=null, IProgress&lt; TaskSnapshot &gt;? progress=null)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a252b3b06dc16279b59ef90c72d89adc2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>CancellationToken</type>
      <name>CancellationToken</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a589ea43db0c906a1411c0cd46f8d3926</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="protected">
      <type>ICredentialProvider?</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a3be988e40e3cb623c851e09fec7cf4db</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>abstract string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a2981f7b9b8eef6f174e862634bd2d300</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>object?</type>
      <name>Tag</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a224fb6243abcf2cff7a1954bd89a433b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>virtual bool</type>
      <name>CanCancel</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>af1746ea1e1bbb92ddf38d96260edaf32</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package">
      <type>TaskState</type>
      <name>State</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a38a0f1ef6cfa70fe837532d601b2af69</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>abstract bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>aeaeda8c31b809aa347012eeb2e0db9a2</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>long</type>
      <name>UnitsProcessed</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a03f56bf220e912170ac761e5c0944a0c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>long</type>
      <name>UnitsTotal</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_base.html</anchorfile>
      <anchor>a9890ee6f24956ec7ad3b3e3c52de2be1</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TaskControl</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_task_control.html</filename>
    <member kind="function">
      <type></type>
      <name>TaskControl</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_control.html</anchorfile>
      <anchor>a4418c7181d874770334440a468bbdafa</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_control.html</anchorfile>
      <anchor>ae8fd19cf1f9679df28fddbde68b6ccef</anchor>
      <arglist>(TaskSnapshot value)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_control.html</anchorfile>
      <anchor>ae443abdb4076e38ec6e48093cd875a20</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>TaskName</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_control.html</anchorfile>
      <anchor>afd186edb56f18c280e7c1221db69988e</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::TaskHandlerBase</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</filename>
    <base>NanoByte::Common::Tasks::ITaskHandler</base>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a41b51f9a9e390cadae9bb3c1856cfce6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>RunTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a205796406e7de7cf26d1815bc8cbf8bd</anchor>
      <arglist>(ITask task)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Ask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a127da6594dd94e62d5cb416bb7e1705a</anchor>
      <arglist>(string question, bool? defaultAnswer=null, string? alternateMessage=null)</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract void</type>
      <name>Output</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a7af6a380ec0820dc028579f16fc39b97</anchor>
      <arglist>(string title, string message)</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>abd1024455b5425111bc0a33ceda9f413</anchor>
      <arglist>(string title, IEnumerable&lt; T &gt; data)</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Output&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a3236c6b9a150851e74b58c9c44bf6192</anchor>
      <arglist>(string title, NamedCollection&lt; T &gt; data)</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract void</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a401ce509e867dd2e11e329c4b7fa9566</anchor>
      <arglist>(Exception exception)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract bool</type>
      <name>AskInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>ad851073250ebefa13eb8e46735756651</anchor>
      <arglist>(string question, bool defaultAnswer)</arglist>
    </member>
    <member kind="property" protection="protected">
      <type>CancellationTokenSource</type>
      <name>CancellationTokenSource</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a17ff0e532e1eaadae9722968ef231287</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>CancellationToken</type>
      <name>CancellationToken</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>af4cd4affb42bb89798fbebd3b4496ec0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>virtual ? ICredentialProvider</type>
      <name>CredentialProvider</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>af60fcfd1e0254854bd24d8aa55f895e2</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>Verbosity</type>
      <name>Verbosity</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>aef4520b4fa58759517e3e1d567f33caa</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>virtual bool</type>
      <name>IsInteractive</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_base.html</anchorfile>
      <anchor>a7f893af3f46709e1619143724e06976c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::TaskHandlerExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>OutputLow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_handler_extensions.html</anchorfile>
      <anchor>a1926cb9ee87733ff7293cd747d423cf2</anchor>
      <arglist>(this ITaskHandler handler, [Localizable(true)] string title, [Localizable(true)] string message)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TaskLabel</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_task_label.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_label.html</anchorfile>
      <anchor>a1d7c2e6fdf5cfc98121871e6ce838f52</anchor>
      <arglist>(TaskSnapshot value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TaskProgressBar</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_task_progress_bar.html</filename>
    <member kind="function">
      <type>void</type>
      <name>Report</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_task_progress_bar.html</anchorfile>
      <anchor>aad18ad1bd922743984eebbd6261941a8</anchor>
      <arglist>(TaskSnapshot value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Tasks::TaskRunDialog</name>
    <filename>class_nano_byte_1_1_common_1_1_tasks_1_1_task_run_dialog.html</filename>
    <member kind="function">
      <type></type>
      <name>TaskRunDialog</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_run_dialog.html</anchorfile>
      <anchor>a80b392c7132a91faec48d3c99e57d118</anchor>
      <arglist>(ITask task, ICredentialProvider? credentialProvider, CancellationTokenSource cancellationTokenSource)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_run_dialog.html</anchorfile>
      <anchor>ae13081b1c2dde3398b1ad35518877399</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="property">
      <type>Exception?</type>
      <name>Exception</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_tasks_1_1_task_run_dialog.html</anchorfile>
      <anchor>a691d1cf740e07bf6233f1b4e5c0d5469</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="struct">
    <name>NanoByte::Common::Tasks::TaskSnapshot</name>
    <filename>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</filename>
    <member kind="function">
      <type></type>
      <name>TaskSnapshot</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a57dda92799e3c3f07c8129a32cf754fb</anchor>
      <arglist>(TaskState state, bool unitsByte=false, long unitsProcessed=0, long unitsTotal=-1)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a67a881efc4e8087e19541f66824183fe</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>TaskState</type>
      <name>State</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>afeb46d20ebe079c08861c4213377a51f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>UnitsByte</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a46a0e4209a8f75594c315c93c85d88a8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>long</type>
      <name>UnitsProcessed</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a8b8f619493a2e3a1a7f60d7dfa6c15e5</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>long</type>
      <name>UnitsTotal</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a77d75c6a0a3f508e45921702715aa882</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>double</type>
      <name>Value</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_tasks_1_1_task_snapshot.html</anchorfile>
      <anchor>a8c64a53b0969d6a85a5433d9136f5708</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::TemporaryDirectory</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_directory.html</filename>
    <member kind="function">
      <type></type>
      <name>TemporaryDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_directory.html</anchorfile>
      <anchor>a27a8ed1713b9ecfd9d2f5981d78e999c</anchor>
      <arglist>([Localizable(false)] string prefix)</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_directory.html</anchorfile>
      <anchor>a30a8360337d74eebec33e5cbcb1f15e9</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_directory.html</anchorfile>
      <anchor>a8d74463ef8e5d60d50b309a56f3d0d0b</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::TemporaryFile</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_file.html</filename>
    <member kind="function">
      <type></type>
      <name>TemporaryFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_file.html</anchorfile>
      <anchor>a0a541fef1f7ca20a44989a198a808b9d</anchor>
      <arglist>([Localizable(false)] string prefix)</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_file.html</anchorfile>
      <anchor>a04d3453b4c24c5d1d60b66078e16f61d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_file.html</anchorfile>
      <anchor>a3e5bf09ef83f722243d3d0c7fa19210b</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::TemporaryFlagFile</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_flag_file.html</filename>
    <base>NanoByte::Common::Storage::TemporaryDirectory</base>
    <member kind="function">
      <type></type>
      <name>TemporaryFlagFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_flag_file.html</anchorfile>
      <anchor>a54bf8becd3e1b0d1a818475952c7e303</anchor>
      <arglist>(string prefix)</arglist>
    </member>
    <member kind="property">
      <type>new string</type>
      <name>Path</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_flag_file.html</anchorfile>
      <anchor>a42545f73089a4fb36f2b0941f8d78f38</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>Set</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_flag_file.html</anchorfile>
      <anchor>a77995d97d4342691ad4591499b7713ed</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::TemporaryWorkingDirectory</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_working_directory.html</filename>
    <base>NanoByte::Common::Storage::TemporaryDirectory</base>
    <member kind="function">
      <type></type>
      <name>TemporaryWorkingDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_working_directory.html</anchorfile>
      <anchor>aebcad8e5f98327d0f135b37a0c2641fe</anchor>
      <arglist>(string prefix)</arglist>
    </member>
    <member kind="function">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_temporary_working_directory.html</anchorfile>
      <anchor>ac9ea1d4549ce7768bae0abe612d52b17</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::ThreadUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_thread_utils.html</filename>
    <member kind="function" static="yes">
      <type>static Thread</type>
      <name>StartAsync</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_thread_utils.html</anchorfile>
      <anchor>a0250ea0b5aa05a643a69ece0fe4a9d98</anchor>
      <arglist>(ThreadStart execute, [Localizable(false)] string? name=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Thread</type>
      <name>StartBackground</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_thread_utils.html</anchorfile>
      <anchor>a8f8491a6740608b7efeb893a2caeb5c8</anchor>
      <arglist>(ThreadStart execute, [Localizable(false)] string? name=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>RunSta</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_thread_utils.html</anchorfile>
      <anchor>afc168c220cdc152e664fd18ca7d15ab7</anchor>
      <arglist>(Action execute)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>RunSta&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_thread_utils.html</anchorfile>
      <anchor>aadd997c9d7d40ad8392e110a32082466</anchor>
      <arglist>(Func&lt; T &gt; execute)</arglist>
    </member>
  </compound>
  <compound kind="struct">
    <name>NanoByte::Common::TimedLogEvent</name>
    <filename>struct_nano_byte_1_1_common_1_1_timed_log_event.html</filename>
    <member kind="function">
      <type></type>
      <name>TimedLogEvent</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_timed_log_event.html</anchorfile>
      <anchor>a6940c363bb6f6c6b1a888be9639c9c8d</anchor>
      <arglist>(string entry)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>struct_nano_byte_1_1_common_1_1_timed_log_event.html</anchorfile>
      <anchor>a45aed7def575e9cb4f96ccc5991cd1a4</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TimeSpanControl</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_time_span_control.html</filename>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_time_span_control.html</anchorfile>
      <anchor>a2badf05b38e837b406c2709971b19496</anchor>
      <arglist>(bool disposing)</arglist>
    </member>
    <member kind="property">
      <type>TimeSpan</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_time_span_control.html</anchorfile>
      <anchor>a9e6bb5eca4c25a526a362fa14e539e14</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::TimeSpanEditor</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_time_span_editor.html</filename>
    <member kind="function">
      <type>override UITypeEditorEditStyle</type>
      <name>GetEditStyle</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_time_span_editor.html</anchorfile>
      <anchor>a21657b38d4b2e7b027b566ed938ce1a8</anchor>
      <arglist>(ITypeDescriptorContext context)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>EditValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_time_span_editor.html</anchorfile>
      <anchor>a0a155c76ce8031abc190d5107e8f5245</anchor>
      <arglist>(ITypeDescriptorContext context, IServiceProvider provider, object value)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TouchEventArgs</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</filename>
    <member kind="variable">
      <type>bool</type>
      <name>Primary</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a37347d07932f5993b2cb1aa57411a385</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>bool</type>
      <name>Palm</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>ab8e2d61032488211d29202d812667775</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>bool</type>
      <name>InRange</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>af146c6ef6bde7d82056399ac2c6f1b25</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable">
      <type>bool</type>
      <name>NoCoalesce</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a83d84f8fc7135c4c19a3cd7432dcd71f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>LocationX</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>af0e8a22c6ebfeb01121ea6d489573afb</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>LocationY</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>ab44d67217a8c879a1f3f7cabcc31a563</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>ContactX</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a27d0d8e5b3497a7ed1b0b042f6ab9969</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>ContactY</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a9d63afb569dd6eaa3dbceae6fa42b669</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>ID</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>aba362e5f33b7df6f8a1ff253925a2b9b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>TouchEventMask</type>
      <name>Mask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a47b6361c43261355a28f0ffa347c9075</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>int</type>
      <name>Time</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_event_args.html</anchorfile>
      <anchor>a1d4c483103633860f759cdddda80a641</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TouchForm</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_touch_form.html</filename>
    <base>NanoByte::Common::Controls::ITouchControl</base>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchDown</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_form.html</anchorfile>
      <anchor>a4f8c5dc882bf768ec52596859b44a194</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchUp</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_form.html</anchorfile>
      <anchor>a69d2149111f0cf155d495c3d5862c206</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchMove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_form.html</anchorfile>
      <anchor>aa396bd456a57f375cebcb8a06171e6d8</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::TouchPanel</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_touch_panel.html</filename>
    <base>NanoByte::Common::Controls::ITouchControl</base>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchDown</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_panel.html</anchorfile>
      <anchor>ada816a1e9310c9c0222f2439ee47bbc8</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchUp</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_panel.html</anchorfile>
      <anchor>a7a1e4efc3e293f1103673a4fae468751</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>EventHandler&lt; TouchEventArgs &gt;?</type>
      <name>TouchMove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_touch_panel.html</anchorfile>
      <anchor>a561613f20c33d2355af385c299f41c6c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::TransparentCache</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::Common::Collections::TransparentCacheBase</base>
    <member kind="function">
      <type></type>
      <name>TransparentCache</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</anchorfile>
      <anchor>a9f360ddf702873fef66e6933b58a2cf8</anchor>
      <arglist>(Func&lt; TKey, TValue &gt; retriever)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override TValue</type>
      <name>Retrieve</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</anchorfile>
      <anchor>a2997af6041dfe6e4d7148da4042ebfb9</anchor>
      <arglist>(TKey key)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>TransparentCache&lt; Uri, NetworkCredential?&gt;</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</filename>
    <member kind="function">
      <type></type>
      <name>TransparentCache</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</anchorfile>
      <anchor>a9f360ddf702873fef66e6933b58a2cf8</anchor>
      <arglist>(Func&lt; TKey, TValue &gt; retriever)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override TValue</type>
      <name>Retrieve</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache.html</anchorfile>
      <anchor>a2997af6041dfe6e4d7148da4042ebfb9</anchor>
      <arglist>(TKey key)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::TransparentCacheBase</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache_base.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type>bool</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache_base.html</anchorfile>
      <anchor>ab025f7c8ae641f86c852a417a617606e</anchor>
      <arglist>(TKey key)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Clear</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache_base.html</anchorfile>
      <anchor>a6f000a2eb44427ccbe0093d8d76730e5</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract TValue</type>
      <name>Retrieve</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache_base.html</anchorfile>
      <anchor>a2643373500467d4fbd27d610f2f6c066</anchor>
      <arglist>(TKey key)</arglist>
    </member>
    <member kind="property">
      <type>TValue</type>
      <name>this[TKey key]</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_transparent_cache_base.html</anchorfile>
      <anchor>a2ca397dcba26c08d83a6f02e8d951b2f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::UnixUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>abc75d761972a1d2be98b1ba6f9c9ff8a</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateHardlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a28161ff88d4a9ce9e49bac8eee93725e</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AreHardlinked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a72bb003b5a7d0b15b6ce648ec3e061d9</anchor>
      <arglist>([Localizable(false)] string path1, [Localizable(false)] string path2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Rename</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a2824793ce07f1129a574509e93c9568f</anchor>
      <arglist>([Localizable(false)] string source, [Localizable(false)] string destination)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsRegularFile</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>adf055b8e06639ca23457b2af17a7641c</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>aa2b3fb52d2521b065c2f7c99767da0e3</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a7e444fa2093e318a8c8ce06f80a4260c</anchor>
      <arglist>([Localizable(false)] string path, [NotNullWhen(true)] out string? target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>MakeReadOnly</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>af6529515f26df604363f6e694f904e8c</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>MakeWritable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>abcf475e9232c7a8db71dbc5bbaf79d09</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsExecutable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>aa58bc8b8572ffcf4c6b736f34658161c</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetExecutable</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>afa0e81371fc7a043983ff352e34ed5be</anchor>
      <arglist>([Localizable(false)] string path, bool executable)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? byte[]</type>
      <name>GetXattr</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a60cb5d811ecf64a183f5e0d30c300205</anchor>
      <arglist>([Localizable(false)] string path, [Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetXattr</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>ac44e30702820f051eaafe9af28aace6e</anchor>
      <arglist>([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetFileSystem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a76fc607331945b49d35d84c125b87b93</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsUnix</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a9b6d478fc051539630f6c6614e3c375d</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsMacOSX</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a9566a030983b940e9beb855f52dcdc77</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>HasGui</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a1608ed55667e1de9a4d31367030ab64a</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>OSName</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>a054908f905f7e1422953d5720ebf7f0c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>CpuType</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_unix_utils.html</anchorfile>
      <anchor>ab3c2570bc0a74fb4325b6ca08fd85f0b</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::UpdateUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_update_utils.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>a0eda4aeab7492727073c02e122949c34</anchor>
      <arglist>(this T value, ref T original, ref bool updated)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>a673023614e8d2c453fac6cfe2575e167</anchor>
      <arglist>(this T value, ref T original, ref bool updated1, ref bool updated2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>ae66b98663236bf2c17d435e843ee8c8c</anchor>
      <arglist>(this string value, ref string original, ref bool updated)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>a1f2d054dde63e813ac7ae989e1c3c168</anchor>
      <arglist>(this string value, ref string original, ref bool updated1, ref bool updated2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>a905ba8da2c08158ae2fbb4eaee521067</anchor>
      <arglist>(this T value, ref T original, [InstantHandle] Action updated)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>To</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>a624a63b7e2e5d9589e5abe29d8d51647</anchor>
      <arglist>(this string value, ref string original, [InstantHandle] Action updated)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Swap&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_update_utils.html</anchorfile>
      <anchor>ae1f96bd77a2b3503ac1b3242b1760d60</anchor>
      <arglist>(ref T value1, ref T value2)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::UriExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ToStringRfc</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</anchorfile>
      <anchor>a6b389b75c2eeee92a0d57ac6bdb77bad</anchor>
      <arglist>(this Uri uri)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Uri</type>
      <name>EnsureTrailingSlash</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</anchorfile>
      <anchor>a321b83bd7bfbe222bc850f669436e53c</anchor>
      <arglist>(this Uri uri)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Uri</type>
      <name>ReparseAsAbsolute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</anchorfile>
      <anchor>a4cb39924f783f4452f66898c616935f4</anchor>
      <arglist>(this Uri uri)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetLocalFileName</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</anchorfile>
      <anchor>a9790d7e5852af293a4bcf93a69e15c04</anchor>
      <arglist>(this Uri uri)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static Uri</type>
      <name>GetBaseUri</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_uri_extensions.html</anchorfile>
      <anchor>ab042a6dcc189c1858127cbda1efcf421</anchor>
      <arglist>(this Uri uri)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Controls::UriTextBox</name>
    <filename>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</filename>
    <base>NanoByte::Common::Controls::HintTextBox</base>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnDragEnter</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a55db028ee398840ca914c5dedd77e127</anchor>
      <arglist>(DragEventArgs drgevent)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>OnDragDrop</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a7263a2e1be3d89befbc35869edc706cb</anchor>
      <arglist>(DragEventArgs drgevent)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual bool</type>
      <name>ValidateUri</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a39a76b9a915d3ccfde459fc0962ed33a</anchor>
      <arglist>(string text)</arglist>
    </member>
    <member kind="property">
      <type>Uri??????</type>
      <name>Uri</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a7afdba2a7ec58407eea64ea7fca9e28c</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>HttpOnly</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>aa6e5922d57ad6a66a73dbf676de1b007</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>AllowRelative</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a7e7a412eda689498ea82f6fe02020765</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>IsValid</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_controls_1_1_uri_text_box.html</anchorfile>
      <anchor>a2e9bc9ebff30920bf8a046fa5590ef80</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Values::Design::ValueTypeConverter</name>
    <filename>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>override bool</type>
      <name>GetCreateInstanceSupported</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a2e44ed7116efe3e95e66a232739e7ff6</anchor>
      <arglist>(ITypeDescriptorContext context)</arglist>
    </member>
    <member kind="function">
      <type>override bool</type>
      <name>GetPropertiesSupported</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>affbfa521b31153d629ef8c6319e199b8</anchor>
      <arglist>(ITypeDescriptorContext context)</arglist>
    </member>
    <member kind="function">
      <type>override PropertyDescriptorCollection</type>
      <name>GetProperties</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a3b6c793d3460a218754c742131405128</anchor>
      <arglist>(ITypeDescriptorContext context, object value, Attribute[] attributes)</arglist>
    </member>
    <member kind="function">
      <type>override bool</type>
      <name>CanConvertTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>aa16451c8098ad0c14de353936a31d245</anchor>
      <arglist>(ITypeDescriptorContext context, Type destinationType)</arglist>
    </member>
    <member kind="function">
      <type>override bool</type>
      <name>CanConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a7a6199348003ccdf1d6d76da43a59e6f</anchor>
      <arglist>(ITypeDescriptorContext context, Type sourceType)</arglist>
    </member>
    <member kind="function">
      <type>override? object</type>
      <name>ConvertTo</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>aaf291ce53426888a449d7ba867bf5cac</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)</arglist>
    </member>
    <member kind="function">
      <type>override? object</type>
      <name>ConvertFrom</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>ae92c96cb246a9cabb469af42bc5bf7ba</anchor>
      <arglist>(ITypeDescriptorContext context, CultureInfo culture, object value)</arglist>
    </member>
    <member kind="function">
      <type>override object</type>
      <name>CreateInstance</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>aec4177dd1a624c6a94bc3aed18d0ba93</anchor>
      <arglist>(ITypeDescriptorContext context, IDictionary propertyValues)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual string</type>
      <name>GetElementSeparator</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a7986e29d2634d3ed9d3fb3d7112b0952</anchor>
      <arglist>(CultureInfo culture)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract ConstructorInfo</type>
      <name>GetConstructor</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a800c5cadd1e2884f40bc86af46dd4122</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract object[]</type>
      <name>GetArguments</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a5a3316ace071b88917d17ab0380cfeb9</anchor>
      <arglist>(T value)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract string[]</type>
      <name>GetValues</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a0ecab4bee1d2ab080c064a11ba68baff</anchor>
      <arglist>(T value, ITypeDescriptorContext context, CultureInfo culture)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract T</type>
      <name>GetObject</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a0fc6b897b9b2f4ac9f04cf2a13b3f2fe</anchor>
      <arglist>(string[] values, CultureInfo culture)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract T</type>
      <name>GetObject</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>afdff8a3032e49b15b2bfb1bf2f161de2</anchor>
      <arglist>(IDictionary propertyValues)</arglist>
    </member>
    <member kind="property" protection="protected">
      <type>abstract int</type>
      <name>NoArguments</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_values_1_1_design_1_1_value_type_converter.html</anchorfile>
      <anchor>a38c02ea1930675f14d37a8dd856f9926</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::WaitHandleExtensions</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_wait_handle_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>WaitOne</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_wait_handle_extensions.html</anchorfile>
      <anchor>afd18f4a8e2b673fd458a846e0207eb96</anchor>
      <arglist>(this WaitHandle handle, CancellationToken cancellationToken, int millisecondsTimeout=-1)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Threading::WaitTask</name>
    <filename>class_nano_byte_1_1_common_1_1_threading_1_1_wait_task.html</filename>
    <base>NanoByte::Common::Tasks::TaskBase</base>
    <member kind="function">
      <type></type>
      <name>WaitTask</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_wait_task.html</anchorfile>
      <anchor>aa1646b09e955f9697f8560ddecdd8c7c</anchor>
      <arglist>([Localizable(true)] string name, WaitHandle waitHandle, int millisecondsTimeout=Timeout.Infinite)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>Execute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_wait_task.html</anchorfile>
      <anchor>a4ba468c290d1e5d2509a336fe455795e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>override string</type>
      <name>Name</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_wait_task.html</anchorfile>
      <anchor>aa47ffe7390e65bb7ff713e1789836a05</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="protected">
      <type>override bool</type>
      <name>UnitsByte</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_threading_1_1_wait_task.html</anchorfile>
      <anchor>a966ecc4292f287afbd9919394199371f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::WebClientTimeout</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_web_client_timeout.html</filename>
    <member kind="function">
      <type></type>
      <name>WebClientTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_web_client_timeout.html</anchorfile>
      <anchor>aabb06beff4315d242f6f85fd3336c0b2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>WebClientTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_web_client_timeout.html</anchorfile>
      <anchor>aee402d846bbc807cc912654097b9418d</anchor>
      <arglist>(int timeout)</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const int</type>
      <name>DefaultTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_web_client_timeout.html</anchorfile>
      <anchor>ae240444d3d1708662563aa992d98417d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::WindowsCliCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_windows_cli_credential_provider.html</filename>
    <base>NanoByte::Common::Net::WindowsCredentialProvider</base>
    <member kind="function" protection="protected">
      <type>override NetworkCredential</type>
      <name>Prompt</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_windows_cli_credential_provider.html</anchorfile>
      <anchor>a8eaa90842c10e96b57555b1542555cba</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::WindowsCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_windows_credential_provider.html</filename>
    <base>NanoByte::Common::Net::CredentialProviderBase</base>
    <member kind="function">
      <type>override? NetworkCredential</type>
      <name>GetCredential</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_windows_credential_provider.html</anchorfile>
      <anchor>a3bb90bfb84e6dd917a201d59cd49e8d3</anchor>
      <arglist>(Uri uri, string authType)</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="pure">
      <type>abstract ? NetworkCredential</type>
      <name>Prompt</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_windows_credential_provider.html</anchorfile>
      <anchor>a30c2114a7f31d8d3b3db8d298b388eb4</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WindowsCredentials</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_windows_credentials.html</filename>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsCredentialStored</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_credentials.html</anchorfile>
      <anchor>a07b70f74f9d18d4e59f8724286812467</anchor>
      <arglist>(string target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static NetworkCredential</type>
      <name>PromptGui</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_credentials.html</anchorfile>
      <anchor>a3e68d5838174e364bd7d718aeedddf93</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags, string? title=null, string? message=null, IntPtr owner=default)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static NetworkCredential</type>
      <name>PromptCli</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_credentials.html</anchorfile>
      <anchor>a433f33a5666d58e82b6a8efce91d7c9c</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::WindowsGuiCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_windows_gui_credential_provider.html</filename>
    <base>NanoByte::Common::Net::WindowsCredentialProvider</base>
    <member kind="function" protection="protected">
      <type>override NetworkCredential</type>
      <name>Prompt</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_windows_gui_credential_provider.html</anchorfile>
      <anchor>ac2158a36ac26f3228996265e76f80aac</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WindowsMutex</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_windows_mutex.html</filename>
    <member kind="function" static="yes">
      <type>static IntPtr</type>
      <name>Create</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_mutex.html</anchorfile>
      <anchor>a01e109e262544616412d4f69835857a7</anchor>
      <arglist>([Localizable(false)] string name, out bool alreadyExists)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>Probe</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_mutex.html</anchorfile>
      <anchor>aa4ad88e4b1f0eafbcf49101851e989a1</anchor>
      <arglist>([Localizable(false)] string name)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>Close</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_mutex.html</anchorfile>
      <anchor>a5e1430fa8c510349cf7454d225c7e97d</anchor>
      <arglist>(IntPtr handle)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WindowsRestartManager</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</filename>
    <member kind="function">
      <type></type>
      <name>WindowsRestartManager</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>acd05dc1244594fcf7f20785fb912f6e5</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>ae9d5745f51e08a8cdfcd7863de7a53a2</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>RegisterResources</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>af7d3f1151d05b680f11b68c021f60d95</anchor>
      <arglist>(params string[] files)</arglist>
    </member>
    <member kind="function">
      <type>string[]</type>
      <name>ListApps</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>a79816d236a6ac026c12c88381440cde5</anchor>
      <arglist>(CancellationToken cancellationToken=default)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>ShutdownApps</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>af6299fa0ca9a731c30560e1e18710def</anchor>
      <arglist>(ITaskHandler handler)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>RestartApps</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_restart_manager.html</anchorfile>
      <anchor>a1bd2de0602b237207e33f2f663f66dc4</anchor>
      <arglist>(ITaskHandler handler)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Net::WindowsSilentCredentialProvider</name>
    <filename>class_nano_byte_1_1_common_1_1_net_1_1_windows_silent_credential_provider.html</filename>
    <base>NanoByte::Common::Net::WindowsCredentialProvider</base>
    <member kind="function" protection="protected">
      <type>override? NetworkCredential</type>
      <name>Prompt</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_net_1_1_windows_silent_credential_provider.html</anchorfile>
      <anchor>a3aa312404908693841147e80b82681e9</anchor>
      <arglist>(string target, WindowsCredentialsFlags flags)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WindowsTaskbar</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</filename>
    <class kind="struct">NanoByte::Common::Native::WindowsTaskbar::ShellLink</class>
    <member kind="enumeration">
      <type></type>
      <name>ProgressBarState</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a082c428a6d981569eb73d408bd0bfdfd</anchor>
      <arglist></arglist>
      <enumvalue file="class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html" anchor="a082c428a6d981569eb73d408bd0bfdfdab644b4f0de4cf0c46acb4583602e168c">NoProgress</enumvalue>
      <enumvalue file="class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html" anchor="a082c428a6d981569eb73d408bd0bfdfdaa2d00c353d1f9a5f07852650030dbd53">Indeterminate</enumvalue>
      <enumvalue file="class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html" anchor="a082c428a6d981569eb73d408bd0bfdfda960b44c579bc2f6818d2daaf9e4c16f0">Normal</enumvalue>
      <enumvalue file="class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html" anchor="a082c428a6d981569eb73d408bd0bfdfda902b0d55fddef6f8d651fe1035b7d4bd">Error</enumvalue>
      <enumvalue file="class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html" anchor="a082c428a6d981569eb73d408bd0bfdfdae99180abf47a8b3a856e0bcb2656990a">Paused</enumvalue>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetProgressState</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a30fe1426a693f1b355041dad35cb9560</anchor>
      <arglist>(IntPtr handle, ProgressBarState state)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetProgressValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a6c4a70038d500bff607f03ac9b71528a</anchor>
      <arglist>(IntPtr handle, int currentValue, int maximumValue)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetWindowAppID</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a7fb3aaaa9307b7400b8229a1e3756954</anchor>
      <arglist>(IntPtr hwnd, [Localizable(false)] string appID, [Localizable(false)] string? relaunchCommand=null, [Localizable(false)] string? relaunchIcon=null, [Localizable(true)] string? relaunchName=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>AddTaskLinks</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a9690db471cf0c61563cb960d37d794c3</anchor>
      <arglist>([Localizable(false)] string appID, IEnumerable&lt; ShellLink &gt; links)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>PreventPinning</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_taskbar.html</anchorfile>
      <anchor>a63a5994b311b1dafc3698dae5c082d4f</anchor>
      <arglist>(IntPtr hwnd)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WindowsUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</filename>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>GetNetFxDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>aafb89a454d65e06f02c2cbdda3c66859</anchor>
      <arglist>(string version)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string[]</type>
      <name>SplitArgs</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>adba560517f0d2c508bded19fce0ccc40</anchor>
      <arglist>(string? commandLine)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AttachConsole</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>abe955a2f7f340418a9defeabe044830d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? byte[]</type>
      <name>ReadAllBytes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ab4aa691733b0d39f7e5f6294b39e6727</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>WriteAllBytes</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ab31fcf8e45094472f117c631797c53ee</anchor>
      <arglist>([Localizable(false)] string path, byte[] data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>addecf0c948ac893237e2f65eafd0fffb</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a17d5bb8644fd711fea5c0c7ba5af7a99</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsSymlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ac30bdd7ffd869ea5b641d962865f6f3c</anchor>
      <arglist>([Localizable(false)] string path, [NotNullWhen(true)] out string? target)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CreateHardlink</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>aa9f68ef23d1fc5c52c49ecf7a1bdf6d6</anchor>
      <arglist>([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>AreHardlinked</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a1b0c65649a890988d9c52b13b3bf820f</anchor>
      <arglist>([Localizable(false)] string path1, [Localizable(false)] string path2)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>MoveFileOnReboot</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a6348f245923691c3afb07f626674e168</anchor>
      <arglist>(string sourcePath, string? destinationPath)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetCurrentProcessAppID</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ac1beb9c4f1067e3e673f0dcea6bf4d55</anchor>
      <arglist>(string appID)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>NotifyAssocChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>aff84a633783dce3fd7a80c8465192dfa</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>NotifyEnvironmentChanged</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a9df9535adcd0665764397e2dd97baaaf</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static int</type>
      <name>RegisterWindowMessage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a672187d19f510bc31b14a15e594353cb</anchor>
      <arglist>([Localizable(false)] string message)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>BroadcastMessage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a59a9ca5233c3132e4c3349f6404066ea</anchor>
      <arglist>(int messageID)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>RegisterApplicationRestart</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a8b3868ba97794257ac3134395d09b229</anchor>
      <arglist>(string arguments)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>UnregisterApplicationRestart</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a99c4281a70d280608b41ed3aa675e0d8</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>NetFx20</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ab0483946b2e75194a46303479339a580</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>NetFx30</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ac69d34134e5de3e3d51d2f60344b284c</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>NetFx35</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a0f13313e05b6bea6489b167e90b61500</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>NetFx40</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a0cbb10595eba49e685111ea6f36df130</anchor>
      <arglist></arglist>
    </member>
    <member kind="function" protection="package" static="yes">
      <type>static Exception</type>
      <name>BuildException</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a148f989faf451bc76b331e494392fc1d</anchor>
      <arglist>(int error)</arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorFileNotFound</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a8b0beb707d9179a99848397b89b51773</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorAccessDenied</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ad328b99ca6c56a3893631881c54c9ff0</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorWriteFault</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a1ead3ad200c4e67e85ccf0d5a3bf2a80</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorSemTimeout</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a243357e92c9e3014883eb3ced78df799</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorAlreadyExists</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a2502db58c819f0a18fa64b946954047f</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorMoreData</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a86c0a4063245b18ff3c5f8b5f99ce2ef</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorRequestedOperationRequiresElevation</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a5ebc39506988b8c7e8d28616ad33d860</anchor>
      <arglist></arglist>
    </member>
    <member kind="variable" protection="package" static="yes">
      <type>const int</type>
      <name>Win32ErrorCancelled</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a8eacd8f4a8191545dfaf41b864b810b0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindows</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a392250cdc038e0b4aefc988f2724673f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindowsNT</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>abc6bba9a0f2b28e50227080eae23efc7</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindowsXP</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ad98d1dff49c77f4769584099a2f03ab4</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindowsVista</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ac4352570a28b81ba87e6ca74400245e9</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindows7</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a50438e9acbe11ccbd241032b6ebb7aad</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindows8</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a610f7809e9a5a33e708bd79e0c0d6a6b</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindows10</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a09900df1623ae2c2bf3292fa06e50870</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsWindows10Redstone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>af2e15bdefa21b3d8ab4fdd599556696e</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>HasUac</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a12bd707db6ad4731da53209f2955efd0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsAdministrator</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>ad804fb098e378dc039d3319f32b1fe7f</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>IsGuiSession</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a237187f7b7fc0d78d5e96eef11ed8f25</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static string</type>
      <name>CurrentProcessPath</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a8ec356d90493eff6f7c0fd2d7b7a4745</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static double</type>
      <name>AbsoluteTime</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_windows_utils.html</anchorfile>
      <anchor>a9cd46fe7c96b34a8221a0e599ce6693c</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Native::WinFormsUtils</name>
    <filename>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</filename>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>CenterOnParent</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>a6632e3f9e1ea35883bba586a8d1bee24</anchor>
      <arglist>(this Form form)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SetForegroundWindow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>ab4cfb1b432dcfda89cdba4811570bf07</anchor>
      <arglist>(this Form form)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>EnableWindowDrag</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>add134f8c83cf0b5bcd8369c88f04c36b</anchor>
      <arglist>(this Control control)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>AddShieldIcon</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>afe2554f9b989961a42a31a015b321268</anchor>
      <arglist>(this Button button)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>IsKeyDown</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>a1afe57730271281a0e79918cb4c8f5aa</anchor>
      <arglist>(Keys key)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IntPtr</type>
      <name>SetCapture</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>acf604568f4883b35dd9efb30c5b79469</anchor>
      <arglist>(IntPtr handle)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static bool</type>
      <name>ReleaseCapture</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>aaf2feaaf7f0e73576b35bc7035a732f6</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>RegisterTouchWindow</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>ae8464b386d2882ebd3bb9dfcf675ee63</anchor>
      <arglist>(Control control)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>HandleTouchMessage</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>a5c481f1ccf1ace868752db66ee7d1ce4</anchor>
      <arglist>(ref Message m, object? sender, EventHandler&lt; TouchEventArgs &gt;? onTouchDown, EventHandler&lt; TouchEventArgs &gt;? onTouchMove, EventHandler&lt; TouchEventArgs &gt;? onTouchUp)</arglist>
    </member>
    <member kind="property" static="yes">
      <type>static bool</type>
      <name>AppIdle</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>a11efb1145669a6bb9e765fc88191e639</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" static="yes">
      <type>static float</type>
      <name>CaretBlinkTime</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_native_1_1_win_forms_utils.html</anchorfile>
      <anchor>a58a1714b81da970cfe81401ba5ba4afc</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::WorkingDirectory</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_working_directory.html</filename>
    <member kind="function">
      <type></type>
      <name>WorkingDirectory</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_working_directory.html</anchorfile>
      <anchor>a203f0042ea58d35c2df336e5f683cbc6</anchor>
      <arglist>(string path)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Dispose</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_working_directory.html</anchorfile>
      <anchor>aa1ba37e2b6e611fe5260d757b1acb85f</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::XmlDictionary</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</filename>
    <base>ICloneable&lt; XmlDictionary &gt;</base>
    <member kind="function">
      <type>void</type>
      <name>Add</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a94a91cd5be9575e78aac5300cfc60800</anchor>
      <arglist>(string key, string value)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a590576813bae697532d57fde50f8dfef</anchor>
      <arglist>(string key)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>ContainsKey</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a2cf060905fd5f3ae101d04624b08e6ae</anchor>
      <arglist>(string key)</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>ContainsValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>ac0e2370b238006b37d40531c90b69ef7</anchor>
      <arglist>(string value)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Sort</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>aa5cc207338bdbd26211b8d43a58b983a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>string</type>
      <name>GetValue</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a414eadd542a230a2d8896dd20b4fb360</anchor>
      <arglist>(string key)</arglist>
    </member>
    <member kind="function">
      <type>IDictionary&lt; string, string &gt;</type>
      <name>ToDictionary</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a8a257986b446a8918393b351fac5d904</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="virtual">
      <type>virtual XmlDictionary</type>
      <name>Clone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a61cd0321e0cb80abe9ad6a237b6aa6a3</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>override void</type>
      <name>InsertItem</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary.html</anchorfile>
      <anchor>a2850fd78355dc56a1e0704b33766b6ec</anchor>
      <arglist>(int index, XmlDictionaryEntry item)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Collections::XmlDictionaryEntry</name>
    <filename>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</filename>
    <base>ICloneable&lt; XmlDictionaryEntry &gt;</base>
    <member kind="function">
      <type></type>
      <name>XmlDictionaryEntry</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a5f01a7d48ded77577bcb495221ab524e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type></type>
      <name>XmlDictionaryEntry</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a35a4aff9b2d4f7896afc555bf8574fcb</anchor>
      <arglist>(string key, string value)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>ToString</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a334284dccc4b2437a6033de36b22f942</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>bool</type>
      <name>Equals</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a6351cc442d9a0b1b79f9d499438b4890</anchor>
      <arglist>(XmlDictionaryEntry? other)</arglist>
    </member>
    <member kind="function">
      <type>override bool</type>
      <name>Equals</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>aa98a1dbe09c65a0c243b890a431ca330</anchor>
      <arglist>(object? obj)</arglist>
    </member>
    <member kind="function">
      <type>override int</type>
      <name>GetHashCode</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a3725d91ab802191f125df906b37ede98</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>XmlDictionaryEntry</type>
      <name>Clone</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a5c3ac0cb12cc24ed4ead50df7aafd7be</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="variable" protection="package">
      <type>XmlDictionary?</type>
      <name>Parent</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>acd4d61152cc926e11330f9710b66b6e5</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string??</type>
      <name>Key</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a98c4354d052f4e47717fed710d1a2175</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>Value</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_collections_1_1_xml_dictionary_entry.html</anchorfile>
      <anchor>a71116af5f1dc97e46240d4ef1904bf68</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::XmlNamespaceAttribute</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_xml_namespace_attribute.html</filename>
    <member kind="function">
      <type></type>
      <name>XmlNamespaceAttribute</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_namespace_attribute.html</anchorfile>
      <anchor>a08eee983e156587bd74f98cc9624ffec</anchor>
      <arglist>(string name, string ns)</arglist>
    </member>
    <member kind="property">
      <type>XmlQualifiedName</type>
      <name>QualifiedName</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_namespace_attribute.html</anchorfile>
      <anchor>a82a35f6d46c84b7764e58b3aef602b6f</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::Common::Storage::XmlStorage</name>
    <filename>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</filename>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadXml&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>ae9d098123c6bb7bddef4ebd7daa0155f</anchor>
      <arglist>(Stream stream)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>LoadXml&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>a1af0c03e9d05faeaf11a44756949a377</anchor>
      <arglist>([Localizable(false)] string path)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static T</type>
      <name>FromXmlString&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>aa8909a67c34d8e3c2b97b4a99622da7a</anchor>
      <arglist>([Localizable(false)] string data)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveXml&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>a129a04698906265943f3497dfa2d694a</anchor>
      <arglist>(this T data, Stream stream, [Localizable(false)] string? stylesheet=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static void</type>
      <name>SaveXml&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>a8e183d2506df4cda153150002f66ccb4</anchor>
      <arglist>(this T data, [Localizable(false)] string path, [Localizable(false)] string? stylesheet=null)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static string</type>
      <name>ToXmlString&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>aa10bf1362e5eacd18432ed93e346670f</anchor>
      <arglist>(this T data, [Localizable(false)] string? stylesheet=null)</arglist>
    </member>
    <member kind="variable" static="yes">
      <type>const string</type>
      <name>XsiNamespace</name>
      <anchorfile>class_nano_byte_1_1_common_1_1_storage_1_1_xml_storage.html</anchorfile>
      <anchor>a32c691ea23dfa74d6e0b42f8eed27fc6</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common</name>
    <filename>namespace_nano_byte_1_1_common.html</filename>
    <namespace>NanoByte::Common::Collections</namespace>
    <namespace>NanoByte::Common::Controls</namespace>
    <namespace>NanoByte::Common::Dispatch</namespace>
    <namespace>NanoByte::Common::Drawing</namespace>
    <namespace>NanoByte::Common::Info</namespace>
    <namespace>NanoByte::Common::Native</namespace>
    <namespace>NanoByte::Common::Net</namespace>
    <namespace>NanoByte::Common::Storage</namespace>
    <namespace>NanoByte::Common::Streams</namespace>
    <namespace>NanoByte::Common::Tasks</namespace>
    <namespace>NanoByte::Common::Threading</namespace>
    <namespace>NanoByte::Common::Undo</namespace>
    <namespace>NanoByte::Common::Values</namespace>
    <class kind="class">NanoByte::Common::Disposable</class>
    <class kind="class">NanoByte::Common::EncodingUtils</class>
    <class kind="class">NanoByte::Common::ExceptionUtils</class>
    <class kind="interface">NanoByte::Common::ICloneable</class>
    <class kind="interface">NanoByte::Common::IHighlightColor</class>
    <class kind="interface">NanoByte::Common::INamed</class>
    <class kind="class">NanoByte::Common::Named</class>
    <class kind="class">NanoByte::Common::Log</class>
    <class kind="class">NanoByte::Common::MathUtils</class>
    <class kind="class">NanoByte::Common::NotAdminException</class>
    <class kind="class">NanoByte::Common::ProcessUtils</class>
    <class kind="class">NanoByte::Common::PropertyPointer</class>
    <class kind="class">NanoByte::Common::StagedOperation</class>
    <class kind="class">NanoByte::Common::StringUtils</class>
    <class kind="struct">NanoByte::Common::TimedLogEvent</class>
    <class kind="class">NanoByte::Common::UpdateUtils</class>
    <class kind="class">NanoByte::Common::AnsiCli</class>
    <member kind="enumeration">
      <type></type>
      <name>LogSeverity</name>
      <anchorfile>namespace_nano_byte_1_1_common.html</anchorfile>
      <anchor>ab741dc89bf449b78c013eda9b0b16536</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common.html" anchor="ab741dc89bf449b78c013eda9b0b16536aa603905470e2a5b8c13e96b579ef0dba">Debug</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common.html" anchor="ab741dc89bf449b78c013eda9b0b16536a4059b0251f66a18cb56f544728796875">Info</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common.html" anchor="ab741dc89bf449b78c013eda9b0b16536a56525ae64d370c0b448ac0d60710ef17">Warn</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common.html" anchor="ab741dc89bf449b78c013eda9b0b16536a902b0d55fddef6f8d651fe1035b7d4bd">Error</enumvalue>
    </member>
    <member kind="function">
      <type>delegate void</type>
      <name>RetryAction</name>
      <anchorfile>namespace_nano_byte_1_1_common.html</anchorfile>
      <anchor>a10017f12bcf8827a6b296ad08790d735</anchor>
      <arglist>(bool lastAttempt)</arglist>
    </member>
    <member kind="function">
      <type>delegate Task</type>
      <name>RetryAsyncAction</name>
      <anchorfile>namespace_nano_byte_1_1_common.html</anchorfile>
      <anchor>a48c18418d4959cc2631877c24db268b7</anchor>
      <arglist>(bool lastAttempt)</arglist>
    </member>
    <member kind="function">
      <type>delegate void</type>
      <name>LogEntryEventHandler</name>
      <anchorfile>namespace_nano_byte_1_1_common.html</anchorfile>
      <anchor>a18ed79649260207b9f69e01bc1cfc4ff</anchor>
      <arglist>(LogSeverity severity, string message)</arglist>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Collections</name>
    <filename>namespace_nano_byte_1_1_common_1_1_collections.html</filename>
    <class kind="class">NanoByte::Common::Collections::ArrayExtensions</class>
    <class kind="class">NanoByte::Common::Collections::CollectionExtensions</class>
    <class kind="class">NanoByte::Common::Collections::ConcurrentSet</class>
    <class kind="class">NanoByte::Common::Collections::CultureComparer</class>
    <class kind="class">NanoByte::Common::Collections::DefaultComparer</class>
    <class kind="class">NanoByte::Common::Collections::DictionaryExtensions</class>
    <class kind="class">NanoByte::Common::Collections::EnumerableExtensions</class>
    <class kind="class">NanoByte::Common::Collections::LanguageSet</class>
    <class kind="class">NanoByte::Common::Collections::ListExtensions</class>
    <class kind="class">NanoByte::Common::Collections::LocalizableString</class>
    <class kind="class">NanoByte::Common::Collections::LocalizableStringCollection</class>
    <class kind="class">NanoByte::Common::Collections::MonitoredCollection</class>
    <class kind="class">NanoByte::Common::Collections::MultiDictionary</class>
    <class kind="class">NanoByte::Common::Collections::NamedCollection</class>
    <class kind="class">NanoByte::Common::Collections::StackExtensions</class>
    <class kind="class">NanoByte::Common::Collections::TransparentCache</class>
    <class kind="class">NanoByte::Common::Collections::TransparentCacheBase</class>
    <class kind="class">NanoByte::Common::Collections::XmlDictionary</class>
    <class kind="class">NanoByte::Common::Collections::XmlDictionaryEntry</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Controls</name>
    <filename>namespace_nano_byte_1_1_common_1_1_controls.html</filename>
    <class kind="class">NanoByte::Common::Controls::ControlExtensions</class>
    <class kind="class">NanoByte::Common::Controls::DropDownButton</class>
    <class kind="class">NanoByte::Common::Controls::ErrorBox</class>
    <class kind="class">NanoByte::Common::Controls::ErrorReportForm</class>
    <class kind="class">NanoByte::Common::Controls::ErrorReport</class>
    <class kind="class">NanoByte::Common::Controls::FilteredTreeView</class>
    <class kind="class">NanoByte::Common::Controls::HintTextBox</class>
    <class kind="interface">NanoByte::Common::Controls::IContextMenu</class>
    <class kind="interface">NanoByte::Common::Controls::IEditorDialog</class>
    <class kind="class">NanoByte::Common::Controls::InputBox</class>
    <class kind="interface">NanoByte::Common::Controls::ITouchControl</class>
    <class kind="class">NanoByte::Common::Controls::Msg</class>
    <class kind="class">NanoByte::Common::Controls::OKCancelDialog</class>
    <class kind="class">NanoByte::Common::Controls::OutputBox</class>
    <class kind="class">NanoByte::Common::Controls::OutputGridBox</class>
    <class kind="class">NanoByte::Common::Controls::OutputTreeBox</class>
    <class kind="class">NanoByte::Common::Controls::PropertyGridForm</class>
    <class kind="class">NanoByte::Common::Controls::ResettablePropertyGrid</class>
    <class kind="class">NanoByte::Common::Controls::RtfBuilder</class>
    <class kind="class">NanoByte::Common::Controls::TaskControl</class>
    <class kind="class">NanoByte::Common::Controls::TaskLabel</class>
    <class kind="class">NanoByte::Common::Controls::TaskProgressBar</class>
    <class kind="class">NanoByte::Common::Controls::TimeSpanControl</class>
    <class kind="class">NanoByte::Common::Controls::TouchEventArgs</class>
    <class kind="class">NanoByte::Common::Controls::TouchForm</class>
    <class kind="class">NanoByte::Common::Controls::TouchPanel</class>
    <class kind="class">NanoByte::Common::Controls::UriTextBox</class>
    <member kind="enumeration">
      <type></type>
      <name>MsgSeverity</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_controls.html</anchorfile>
      <anchor>aa392e950de971e4ccef2fc18ffeddf1c</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="aa392e950de971e4ccef2fc18ffeddf1ca4059b0251f66a18cb56f544728796875">Info</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="aa392e950de971e4ccef2fc18ffeddf1ca56525ae64d370c0b448ac0d60710ef17">Warn</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="aa392e950de971e4ccef2fc18ffeddf1ca902b0d55fddef6f8d651fe1035b7d4bd">Error</enumvalue>
    </member>
    <member kind="enumeration">
      <type></type>
      <name>RtfColor</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_controls.html</anchorfile>
      <anchor>ad583cc66cf4c71a65e821ee1b35063e4</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4ae90dfb84e30edf611e326eeb04d680de">Black</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4a9594eec95be70e7b1710f730fdda33d9">Blue</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4ad382816a3cbeed082c9e216e7392eed1">Green</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4a51e6cd92b6c45f9affdc158ecca2b8b8">Yellow</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4a909cea0c97058cfe2e3ea8d675cb08e1">Orange</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="ad583cc66cf4c71a65e821ee1b35063e4aee38e4d5dd68c4e440825018d549cb47">Red</enumvalue>
    </member>
    <member kind="enumeration">
      <type></type>
      <name>TouchEventMask</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_controls.html</anchorfile>
      <anchor>a4f906c36b6770a2133a3cd9cf6d2c543</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="a4f906c36b6770a2133a3cd9cf6d2c543aa76d4ef5f3f6a672bbfab2865563e530">Time</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="a4f906c36b6770a2133a3cd9cf6d2c543abb26f68349d4ba82122bb9acc796c3f2">ExtraInfo</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_controls.html" anchor="a4f906c36b6770a2133a3cd9cf6d2c543aecf5604bd86625e9eea03301a3d85812">ContactArea</enumvalue>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Dispatch</name>
    <filename>namespace_nano_byte_1_1_common_1_1_dispatch.html</filename>
    <class kind="class">NanoByte::Common::Dispatch::AggregateDispatcher</class>
    <class kind="class">NanoByte::Common::Dispatch::Bucketizer</class>
    <class kind="class">NanoByte::Common::Dispatch::BucketRule</class>
    <class kind="interface">NanoByte::Common::Dispatch::IChangeNotify</class>
    <class kind="interface">NanoByte::Common::Dispatch::IMergeable</class>
    <class kind="class">NanoByte::Common::Dispatch::Merge</class>
    <class kind="class">NanoByte::Common::Dispatch::ModelViewSync</class>
    <class kind="class">NanoByte::Common::Dispatch::PerTypeDispatcher</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Drawing</name>
    <filename>namespace_nano_byte_1_1_common_1_1_drawing.html</filename>
    <class kind="class">NanoByte::Common::Drawing::ImageExtensions</class>
    <class kind="class">NanoByte::Common::Drawing::ScalableImage</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Info</name>
    <filename>namespace_nano_byte_1_1_common_1_1_info.html</filename>
    <class kind="struct">NanoByte::Common::Info::AppInfo</class>
    <class kind="class">NanoByte::Common::Info::ExceptionInfo</class>
    <class kind="struct">NanoByte::Common::Info::OSInfo</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Native</name>
    <filename>namespace_nano_byte_1_1_common_1_1_native.html</filename>
    <class kind="class">NanoByte::Common::Native::AppMutex</class>
    <class kind="class">NanoByte::Common::Native::CygwinUtils</class>
    <class kind="class">NanoByte::Common::Native::OSUtils</class>
    <class kind="class">NanoByte::Common::Native::RegistryUtils</class>
    <class kind="class">NanoByte::Common::Native::UnixUtils</class>
    <class kind="class">NanoByte::Common::Native::WindowsCredentials</class>
    <class kind="class">NanoByte::Common::Native::WindowsMutex</class>
    <class kind="class">NanoByte::Common::Native::WindowsRestartManager</class>
    <class kind="class">NanoByte::Common::Native::WindowsTaskbar</class>
    <class kind="class">NanoByte::Common::Native::WindowsUtils</class>
    <class kind="class">NanoByte::Common::Native::WinFormsUtils</class>
    <member kind="enumeration">
      <type></type>
      <name>WindowMessage</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_native.html</anchorfile>
      <anchor>ac975b223f65ec6c7c7034603cc572a6d</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dace2c8aed9c2fa0cfbed56cbda4d8bf07">Empty</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da0e181f89f47654b86f3beb42f5cc08b8">Destroy</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dad3d2e617335f08df83599665eef8a418">Close</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da0d82790b0612935992bd564a17ce37d6">Quit</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da4802a5ac6005a6ab9c68a2fb29e30a3e">Paint</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da2eab09eaf1c33242482fef126247a8ee">SetCursor</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6daad0027b0977719d8e57a95edd42d0e4e">ActivateApplication</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dace4d948d84e68316250b4e87cd760ec3">EnterMenuLoop</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da06abfbe0e0d5cf7fa6807fc3e43fd5db">ExitMenuLoop</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da1d8e9fe1a2d174c047e7de28e0e1f40e">NonClientHitTest</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dadcd064d9ce5519b0f68a13bcc2d83578">PowerBroadcast</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dae47ed8897cb6f9cc675c2cf5feb1d4a9">SystemCommand</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da1ddc1b44b6ec7c62b92a76832b90f438">GetMinMax</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dacfd07bf1effd88bca04a12a087777354">KeyDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da0f8baa14654b1f6ef00fed708c7f198a">KeyUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da76a40e4f974fd895a0a2598c1cee28b4">Character</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da523432fb6c6b9e8184fa690027374a45">SystemKeyDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da38300527a894d9ad2605540cf1d23887">SystemKeyUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dae33654ecca0d7ceb96cdb049ea519702">SystemCharacter</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dafd060c0c157323f1e43e0704d4f3ffc8">MouseMove</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da9d295e0b18c2b3622f770ad4f341c001">LeftButtonDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da226eee9d1b5f329f9fea5cf75d7ed63c">LeftButtonUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da5e71be8560749c6ae20e54c2aeb747cd">LeftButtonDoubleClick</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dab98a35fd6abab7061ed34a295b019623">RightButtonDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da5404f605395f7cea09bc816716605827">RightButtonUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6daeb5fc13295e12d61422ccf6a6059a2ee">RightButtonDoubleClick</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da71dc1c3ad7e5db90aaa7406b296cbcb9">MiddleButtonDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6daa88c7b6ff6a0aa137e81372daa58fe19">MiddleButtonUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da695f8e9abd1d86886e125fab4efccf89">MiddleButtonDoubleClick</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da6924f81750ac471df87fed683bcea516">MouseWheel</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6daa082ce90651cce6b6240422a9713c155">XButtonDown</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da761b2c83c1288d66b32b363f62477c5a">XButtonUp</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da96104585ff497593a7c73f8cde4e90ec">XButtonDoubleClick</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6dac85435094b8a8aa4d1d733509268cb12">MouseFirst</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da047fe92aa5455f57267f65798ec3ee9e">MouseLast</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da17ebd618eba831e5f979d62a54089b40">EnterSizeMove</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da802c5d555dc91a3fac099c3782c82546">ExitSizeMove</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_native.html" anchor="ac975b223f65ec6c7c7034603cc572a6da6f6cb72d544962fa333e2e34ce64f719">Size</enumvalue>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Net</name>
    <filename>namespace_nano_byte_1_1_common_1_1_net.html</filename>
    <class kind="class">NanoByte::Common::Net::CachedCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::ConfigurationCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::ConfigurationCredentialProviderRegistration</class>
    <class kind="class">NanoByte::Common::Net::CredentialProviderBase</class>
    <class kind="class">NanoByte::Common::Net::DownloadFile</class>
    <class kind="interface">NanoByte::Common::Net::ICredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::MicroServer</class>
    <class kind="class">NanoByte::Common::Net::NetUtils</class>
    <class kind="class">NanoByte::Common::Net::UriExtensions</class>
    <class kind="class">NanoByte::Common::Net::WebClientTimeout</class>
    <class kind="class">NanoByte::Common::Net::WindowsCliCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::WindowsCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::WindowsGuiCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::WindowsSilentCredentialProvider</class>
    <class kind="class">NanoByte::Common::Net::AnsiCliCredentialProvider</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Storage</name>
    <filename>namespace_nano_byte_1_1_common_1_1_storage.html</filename>
    <class kind="class">NanoByte::Common::Storage::AtomicRead</class>
    <class kind="class">NanoByte::Common::Storage::AtomicWrite</class>
    <class kind="class">NanoByte::Common::Storage::BinaryStorage</class>
    <class kind="class">NanoByte::Common::Storage::CopyDirectory</class>
    <class kind="class">NanoByte::Common::Storage::FileUtils</class>
    <class kind="class">NanoByte::Common::Storage::JsonStorage</class>
    <class kind="class">NanoByte::Common::Storage::Locations</class>
    <class kind="class">NanoByte::Common::Storage::LocationsRedirect</class>
    <class kind="class">NanoByte::Common::Storage::MoveDirectory</class>
    <class kind="class">NanoByte::Common::Storage::Paths</class>
    <class kind="class">NanoByte::Common::Storage::ReadFile</class>
    <class kind="class">NanoByte::Common::Storage::TemporaryDirectory</class>
    <class kind="class">NanoByte::Common::Storage::TemporaryFile</class>
    <class kind="class">NanoByte::Common::Storage::TemporaryFlagFile</class>
    <class kind="class">NanoByte::Common::Storage::TemporaryWorkingDirectory</class>
    <class kind="class">NanoByte::Common::Storage::WorkingDirectory</class>
    <class kind="class">NanoByte::Common::Storage::XmlNamespaceAttribute</class>
    <class kind="class">NanoByte::Common::Storage::XmlStorage</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Streams</name>
    <filename>namespace_nano_byte_1_1_common_1_1_streams.html</filename>
    <class kind="class">NanoByte::Common::Streams::ChildProcess</class>
    <class kind="class">NanoByte::Common::Streams::DelegatingStream</class>
    <class kind="class">NanoByte::Common::Streams::ExtraDisposeStream</class>
    <class kind="class">NanoByte::Common::Streams::OffsetStream</class>
    <class kind="class">NanoByte::Common::Streams::ProducerConsumerStream</class>
    <class kind="class">NanoByte::Common::Streams::ProgressStream</class>
    <class kind="class">NanoByte::Common::Streams::ShadowingStream</class>
    <class kind="class">NanoByte::Common::Streams::StreamConsumer</class>
    <class kind="class">NanoByte::Common::Streams::StreamUtils</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Tasks</name>
    <filename>namespace_nano_byte_1_1_common_1_1_tasks.html</filename>
    <class kind="class">NanoByte::Common::Tasks::CliProgress</class>
    <class kind="class">NanoByte::Common::Tasks::CliTaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::ForEachTask</class>
    <class kind="interface">NanoByte::Common::Tasks::ITask</class>
    <class kind="interface">NanoByte::Common::Tasks::ITaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::ServiceTaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::SilentTaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::SimplePercentTask</class>
    <class kind="class">NanoByte::Common::Tasks::SimpleTask</class>
    <class kind="class">NanoByte::Common::Tasks::TaskBase</class>
    <class kind="class">NanoByte::Common::Tasks::TaskHandlerBase</class>
    <class kind="class">NanoByte::Common::Tasks::TaskHandlerExtensions</class>
    <class kind="struct">NanoByte::Common::Tasks::TaskSnapshot</class>
    <class kind="class">NanoByte::Common::Tasks::AnsiCliProgress</class>
    <class kind="class">NanoByte::Common::Tasks::AnsiCliProgressContext</class>
    <class kind="class">NanoByte::Common::Tasks::AnsiCliTaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::DialogTaskHandler</class>
    <class kind="class">NanoByte::Common::Tasks::GuiTaskHandlerBase</class>
    <class kind="class">NanoByte::Common::Tasks::TaskRunDialog</class>
    <member kind="enumeration">
      <type></type>
      <name>TaskState</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_tasks.html</anchorfile>
      <anchor>a5bbdbbbe31fcd00dbaae72fb7a898476</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476ae7d31fc0602fb2ede144d18cdffd816b">Ready</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476a8428552d86c0d262a542a528af490afa">Started</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476abf50d5e661106d0abe925af3c2e6f7e7">Header</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476af6068daa29dbb05a7ead1e3b5a48bbee">Data</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476aae94f80b3ce82062a5dd7815daa04f9d">Complete</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476a20484a17a5b571df3649392a6819ff25">WebError</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476a5206bd7472156351d2d9a99633ac9580">IOError</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="a5bbdbbbe31fcd00dbaae72fb7a898476a0e22fe7d45f8e5632a4abf369b24e29c">Canceled</enumvalue>
    </member>
    <member kind="enumeration">
      <type></type>
      <name>Verbosity</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_tasks.html</anchorfile>
      <anchor>aa61f1fee845dd22fbb1dce83280ddc45</anchor>
      <arglist></arglist>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="aa61f1fee845dd22fbb1dce83280ddc45a51ffe9dd1b1e143c1b9f1144d040e454">Batch</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="aa61f1fee845dd22fbb1dce83280ddc45a960b44c579bc2f6818d2daaf9e4c16f0">Normal</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="aa61f1fee845dd22fbb1dce83280ddc45ad4a9fa383ab700c5bdd6f31cf7df0faf">Verbose</enumvalue>
      <enumvalue file="namespace_nano_byte_1_1_common_1_1_tasks.html" anchor="aa61f1fee845dd22fbb1dce83280ddc45aa603905470e2a5b8c13e96b579ef0dba">Debug</enumvalue>
    </member>
    <member kind="function">
      <type>delegate void</type>
      <name>PercentProgressCallback</name>
      <anchorfile>namespace_nano_byte_1_1_common_1_1_tasks.html</anchorfile>
      <anchor>ae5740f2a3fc41390510b38e36fa49740</anchor>
      <arglist>(int percent)</arglist>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Threading</name>
    <filename>namespace_nano_byte_1_1_common_1_1_threading.html</filename>
    <class kind="class">NanoByte::Common::Threading::ActionExtensions</class>
    <class kind="class">NanoByte::Common::Threading::CancellationGuard</class>
    <class kind="class">NanoByte::Common::Threading::FuncExtensions</class>
    <class kind="class">NanoByte::Common::Threading::MarshalNoTimeout</class>
    <class kind="class">NanoByte::Common::Threading::MutexLock</class>
    <class kind="class">NanoByte::Common::Threading::SynchronousProgress</class>
    <class kind="class">NanoByte::Common::Threading::ThreadUtils</class>
    <class kind="class">NanoByte::Common::Threading::WaitHandleExtensions</class>
    <class kind="class">NanoByte::Common::Threading::WaitTask</class>
    <class kind="class">NanoByte::Common::Threading::AsyncFormWrapper</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Undo</name>
    <filename>namespace_nano_byte_1_1_common_1_1_undo.html</filename>
    <class kind="class">NanoByte::Common::Undo::AddToCollection</class>
    <class kind="class">NanoByte::Common::Undo::CollectionCommand</class>
    <class kind="class">NanoByte::Common::Undo::CommandCollector</class>
    <class kind="class">NanoByte::Common::Undo::CommandManager</class>
    <class kind="class">NanoByte::Common::Undo::CompositeCommand</class>
    <class kind="class">NanoByte::Common::Undo::FirstExecuteCommand</class>
    <class kind="interface">NanoByte::Common::Undo::ICommandExecutor</class>
    <class kind="interface">NanoByte::Common::Undo::ICommandManager</class>
    <class kind="interface">NanoByte::Common::Undo::IUndoCommand</class>
    <class kind="interface">NanoByte::Common::Undo::IValueCommand</class>
    <class kind="class">NanoByte::Common::Undo::PreExecutedCommand</class>
    <class kind="class">NanoByte::Common::Undo::PreExecutedCompositeCommand</class>
    <class kind="class">NanoByte::Common::Undo::RemoveFromCollection</class>
    <class kind="class">NanoByte::Common::Undo::ReplaceInList</class>
    <class kind="class">NanoByte::Common::Undo::SetInList</class>
    <class kind="class">NanoByte::Common::Undo::SetLocalizableString</class>
    <class kind="class">NanoByte::Common::Undo::SetValueCommand</class>
    <class kind="class">NanoByte::Common::Undo::SimpleCommand</class>
    <class kind="class">NanoByte::Common::Undo::SimpleCommandExecutor</class>
    <class kind="class">NanoByte::Common::Undo::MultiPropertyChangedCommand</class>
    <class kind="class">NanoByte::Common::Undo::MultiPropertyTracker</class>
    <class kind="class">NanoByte::Common::Undo::PropertyChangedCommand</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Values</name>
    <filename>namespace_nano_byte_1_1_common_1_1_values.html</filename>
    <namespace>NanoByte::Common::Values::Design</namespace>
    <class kind="class">NanoByte::Common::Values::AttributeUtils</class>
    <class kind="class">NanoByte::Common::Values::EnumExtensions</class>
    <class kind="class">NanoByte::Common::Values::KeyEqualityComparer</class>
    <class kind="class">NanoByte::Common::Values::Languages</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::Common::Values::Design</name>
    <filename>namespace_nano_byte_1_1_common_1_1_values_1_1_design.html</filename>
    <class kind="class">NanoByte::Common::Values::Design::EnumDescriptionConverter</class>
    <class kind="class">NanoByte::Common::Values::Design::EnumXmlConverter</class>
    <class kind="class">NanoByte::Common::Values::Design::StringConstructorConverter</class>
    <class kind="class">NanoByte::Common::Values::Design::ValueTypeConverter</class>
    <class kind="class">NanoByte::Common::Values::Design::LanguageSetEditor</class>
    <class kind="class">NanoByte::Common::Values::Design::TimeSpanEditor</class>
  </compound>
  <compound kind="page">
    <name>index</name>
    <title></title>
    <filename>index.html</filename>
    <docanchor file="index.html">md_D__a_common_common_doc_main</docanchor>
  </compound>
</tagfile>

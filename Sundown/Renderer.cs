using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sundown
{

	delegate void mkd_renderer_blockcode (ref IntPtr ob, ref IntPtr text, ref IntPtr lang, IntPtr opaque);
	delegate void mkd_renderer_blockquote(ref IntPtr ob, ref IntPtr text,                  IntPtr opaque);
	delegate void mkd_renderer_blockhtml (ref IntPtr ob, ref IntPtr text,                  IntPtr opaque);
	delegate void mkd_renderer_header    (ref IntPtr ob, ref IntPtr text, int level,       IntPtr opaque);
	delegate void mkd_renderer_hrule     (ref IntPtr ob,                                   IntPtr opaque);
	delegate void mkd_renderer_list      (ref IntPtr ob, ref IntPtr text, int flags,       IntPtr opaque);
	delegate void mkd_renderer_listitem  (ref IntPtr ob, ref IntPtr text, int flags,       IntPtr opaque);
	delegate void mkd_renderer_paragraph (ref IntPtr ob, ref IntPtr text,                  IntPtr opaque);
	delegate void mkd_renderer_table     (ref IntPtr ob, ref IntPtr head, ref IntPtr body, IntPtr opaque);
	delegate void mkd_renderer_table_row (ref IntPtr ob, ref IntPtr text,                  IntPtr opaque);
	delegate void mkd_renderer_table_cell(ref IntPtr ob, ref IntPtr text, int flags,       IntPtr opaque);

	delegate int mkd_renderer_autolink       (ref IntPtr ob, ref IntPtr link, int type, IntPtr opaque);
	delegate int mkd_renderer_codespan       (ref IntPtr ob, ref IntPtr text,           IntPtr opaque);
	delegate int mkd_renderer_double_emphasis(ref IntPtr ob, ref IntPtr text,           IntPtr opaque);
	delegate int mkd_renderer_emphasis       (ref IntPtr ob, ref IntPtr text,           IntPtr opaque);
	delegate int mkd_renderer_image          (ref IntPtr ob, ref IntPtr link, ref IntPtr title, ref IntPtr alt, IntPtr opaque);
	delegate int mkd_renderer_linebreak      (ref IntPtr ob,                            IntPtr opaque);
	delegate int mkd_renderer_link           (ref IntPtr ob, ref IntPtr link, ref IntPtr title, ref IntPtr content, IntPtr opaque);
	delegate int mkd_renderer_raw_html_tag   (ref IntPtr ob, ref IntPtr tag,            IntPtr opaque);
	delegate int mkd_renderer_triple_emphasis(ref IntPtr ob, ref IntPtr text,           IntPtr opaque);
	delegate int mkd_renderer_strikethrough  (ref IntPtr ob, ref IntPtr text,           IntPtr opaque);
	delegate int mkd_renderer_superscript    (ref IntPtr ob, ref IntPtr text,           IntPtr opaque);

	delegate void mkd_renderer_entity(ref IntPtr ob, ref IntPtr entity, IntPtr opaque);
	delegate void mkd_renderer_normal_text(ref IntPtr ob, ref IntPtr text, IntPtr opaque);

	delegate void mkd_renderer_doc_header(ref IntPtr ob, IntPtr opaque);
	delegate void mkd_renderer_doc_footer(ref IntPtr ob, IntPtr opaque);

    [StructLayout(LayoutKind.Sequential)]
    internal struct mkd_renderer
	{
		public mkd_renderer_blockcode  blockcode;
		public mkd_renderer_blockquote blockquote;
		public mkd_renderer_blockhtml  blockhtml;
		public mkd_renderer_header     header;
		public mkd_renderer_hrule      hrule;
		public mkd_renderer_list       list;
		public mkd_renderer_listitem   listitem;
		public mkd_renderer_paragraph  paragrah;
		public mkd_renderer_table      table;
		public mkd_renderer_table_row  table_row;
		public mkd_renderer_table_cell table_cell;


		public mkd_renderer_autolink        autolink;
		public mkd_renderer_codespan        codespan;
		public mkd_renderer_double_emphasis dobule_emphasis;
		public mkd_renderer_emphasis        emphasis;
		public mkd_renderer_image           image;
		public mkd_renderer_linebreak       linebreak;
		public mkd_renderer_link            link;
		public mkd_renderer_raw_html_tag    raw_html_tag;
		public mkd_renderer_triple_emphasis triple_emphasis;
		public mkd_renderer_strikethrough   strikethrough;
		public mkd_renderer_superscript     superscript;

		public mkd_renderer_entity entity;
		public mkd_renderer_normal_text normal_text;

		public mkd_renderer_doc_header doc_header;
		public mkd_renderer_doc_footer doc_footer;

		public IntPtr opaque;
	}

	public abstract class Renderer
	{
		public static Version LibraryVersion {
			get {
				int major, minor, revision;
				sd_version(out major, out minor, out revision);
				return new Version(major, minor, revision);
			}
		}

		internal void Markdown(Buffer outBuffer, Buffer inBuffer, ref mkd_renderer renderer)
		{
			uint i = 0;
			sd_markdown(outBuffer.buf, inBuffer.buf, ref renderer, ~i);
		}

		[DllImport("sundown")]
		internal static extern void sd_markdown(IntPtr outBuffer, IntPtr inBuffer, ref mkd_renderer renderer, uint extensions);

		[DllImport("sundown")]
		internal static extern void sd_version(out int major, out int minor, out int revision);
	}

	// TODO: implement this
	public class CustomRenderer : Renderer
	{
		public CustomRenderer()
		{
		}

		public void Markdown(Buffer outBuffer, Buffer inBuffer)
		{
		}
	}

	public class HtmlRenderer : Renderer, IDisposable
	{
		mkd_renderer renderer;

		public HtmlRenderer()
		{
			renderer = new mkd_renderer();
			sdhtml_renderer(ref renderer, 0, IntPtr.Zero);
		}

		public void Markdown(Buffer outBuffer, Buffer inBuffer)
		{
			Markdown(outBuffer, inBuffer, ref renderer);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool dispose)
		{
			sdhtml_free_renderer(ref renderer);
		}

		[DllImport("sundown")]
		internal static extern void sdhtml_renderer(ref mkd_renderer renderer, int size, IntPtr ptr);

		[DllImport("sundown")]
		internal static extern void sdhtml_free_renderer(ref mkd_renderer renderer);

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodSaver
{
    public partial class SellerMessages : Page
    {
        private class Conversation
        {
            public string ConversationId { get; set; }
            public string CustomerName { get; set; }
            public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        }

        private class ChatMessage
        {
            public string MessageId { get; set; }
            public string Sender { get; set; } // "Customer" or "Seller"
            public DateTime CreatedAt { get; set; }
            public string Body { get; set; }
            public bool IsEdited { get; set; }
        }

        private const string SessionKey = "SellerChats";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerAuthenticated"] as bool? != true)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                EnsureSeedData();
                BindConversationList();
                ResetComposer();
                lblStatus.Text = "Demo mode (Session-based)";
            }
        }

        // Only seller-side needs “real” timing => new seller messages use DateTime.Now (already done below).
        protected string FormatTs(DateTime dt)
        {
            // Chat-app style: today shows time, older shows date+time
            if (dt.Date == DateTime.Today) return dt.ToString("HH:mm");
            return dt.ToString("dd MMM HH:mm");
        }

        private void EnsureSeedData()
        {
            if (Session[SessionKey] != null) return;

            // Seed times can be whatever. They’re demo-only.
            var chats = new List<Conversation>
            {
                new Conversation
                {
                    ConversationId = "C1",
                    CustomerName = "Alicia Tan",
                    Messages = new List<ChatMessage>
                    {
                        new ChatMessage {
                            MessageId = Guid.NewGuid().ToString("N"),
                            Sender = "Customer",
                            CreatedAt = DateTime.Today.AddHours(11).AddMinutes(20),
                            Body = "Hi! Can I collect my order at 6:30pm today?"
                        },
                        new ChatMessage {
                            MessageId = Guid.NewGuid().ToString("N"),
                            Sender = "Seller",
                            CreatedAt = DateTime.Today.AddHours(11).AddMinutes(33),
                            Body = "Hi Alicia! Yes, 6:30pm works. Please show your order confirmation at pickup.",
                            IsEdited = false
                        }
                    }
                },
                new Conversation
                {
                    ConversationId = "C2",
                    CustomerName = "Marcus Lim",
                    Messages = new List<ChatMessage>
                    {
                        new ChatMessage {
                            MessageId = Guid.NewGuid().ToString("N"),
                            Sender = "Customer",
                            CreatedAt = DateTime.Today.AddDays(-1).AddHours(19).AddMinutes(5),
                            Body = "Hello, is the bento still available? I saw it listed just now."
                        }
                    }
                }
            };

            Session[SessionKey] = chats;
        }

        private List<Conversation> GetChats()
        {
            EnsureSeedData();
            return (List<Conversation>)Session[SessionKey];
        }

        private Conversation GetSelectedConversation()
        {
            var id = lstConversations.SelectedValue;
            if (string.IsNullOrWhiteSpace(id)) return null;
            return GetChats().FirstOrDefault(c => c.ConversationId == id);
        }

        private void BindConversationList()
        {
            var chats = GetChats();

            lstConversations.DataSource = chats
                .Select(c => new { c.ConversationId, Display = c.CustomerName })
                .ToList();

            lstConversations.DataValueField = "ConversationId";
            lstConversations.DataTextField = "Display";
            lstConversations.DataBind();

            if (lstConversations.Items.Count > 0)
            {
                lstConversations.SelectedIndex = 0;
                BindThread();
            }
        }

        private void BindThread()
        {
            lblError.Text = "";
            var convo = GetSelectedConversation();

            if (convo == null)
            {
                lblConversationTitle.Text = "Select a conversation";
                rptMessages.DataSource = null;
                rptMessages.DataBind();
                return;
            }

            lblConversationTitle.Text = $"Chat with {convo.CustomerName}";

            rptMessages.DataSource = convo.Messages
                .OrderBy(m => m.CreatedAt)
                .ToList();
            rptMessages.DataBind();

            ResetComposer();
        }

        protected void lstConversations_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindThread();
        }

        protected void btnSendOrUpdate_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            var convo = GetSelectedConversation();
            if (convo == null)
            {
                lblError.Text = "Please select a conversation first.";
                return;
            }

            var text = (txtMessage.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                lblError.Text = "Message cannot be empty.";
                return;
            }

            var editingId = (hfEditingMessageId.Value ?? "").Trim();

            if (string.IsNullOrWhiteSpace(editingId))
            {
                // CREATE (Seller timing reflects real life)
                convo.Messages.Add(new ChatMessage
                {
                    MessageId = Guid.NewGuid().ToString("N"),
                    Sender = "Seller",
                    CreatedAt = DateTime.Now,
                    Body = text,
                    IsEdited = false
                });
            }
            else
            {
                // UPDATE
                var msg = convo.Messages.FirstOrDefault(m => m.MessageId == editingId);
                if (msg == null || msg.Sender != "Seller")
                {
                    lblError.Text = "Unable to edit this message.";
                    ResetComposer();
                    BindThread();
                    return;
                }

                msg.Body = text;
                msg.IsEdited = true;
            }

            txtMessage.Text = "";
            ResetComposer();
            BindThread();
        }

        protected void btnEditHidden_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            var convo = GetSelectedConversation();
            if (convo == null) return;

            var id = (hfContextMsgId.Value ?? "").Trim();
            var msg = convo.Messages.FirstOrDefault(m => m.MessageId == id);

            if (msg == null || msg.Sender != "Seller")
            {
                lblError.Text = "Only seller messages can be edited.";
                return;
            }

            hfEditingMessageId.Value = msg.MessageId;
            txtMessage.Text = msg.Body;

            btnSendOrUpdate.Text = "Update";
            btnCancelEdit.Visible = true;
            lblComposerMode.Text = "Editing selected seller message...";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetComposer();
            txtMessage.Text = "";
        }

        private void ResetComposer()
        {
            hfEditingMessageId.Value = "";
            btnSendOrUpdate.Text = "Send";
            btnCancelEdit.Visible = false;
            lblComposerMode.Text = "Send a message as the seller.";
        }
    }
}
